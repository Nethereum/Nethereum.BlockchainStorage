using Nethereum.BlockchainProcessing.Handlers;
using Nethereum.BlockchainProcessing.Processing;
using Nethereum.BlockchainProcessing.Processors.Transactions;
using Nethereum.RPC.Eth.DTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Nethereum.BlockchainProcessing.BlockchainProxy;
using Nethereum.BlockchainProcessing.Nethereum.RPC.Eth.DTOs;
using Nethereum.RPC.Eth.Services;
using Nethereum.Hex.HexTypes;
using Nethereum.Contracts.Services;
using Nethereum.Web3;

namespace Nethereum.BlockchainProcessing.Processors
{
    public class BlockProcessor : IBlockProcessor
    {
        protected IEthApiContractService BlockProxy { get; }
        protected IBlockHandler BlockHandler { get; }
        protected IEnumerable<IBlockFilter> BlockFilters { get; }
        protected ITransactionProcessor TransactionProcessor { get; }

        private readonly UniqueTransactionHashList _processedTransactions = new UniqueTransactionHashList();
        private ulong _lastBlock;
        private readonly object _sync = new object();

        public BlockProcessor(
            IWeb3 web3, 
            IBlockHandler blockHandler,
            ITransactionProcessor transactionProcessor, 
            IEnumerable<IBlockFilter> blockFilters = null
           )
        {
            BlockProxy = web3.Eth;
            BlockHandler = blockHandler;
            TransactionProcessor = transactionProcessor;
            BlockFilters = blockFilters ?? new IBlockFilter[0];
        }

        public bool ProcessTransactionsInParallel { get; set; } = true;

        public virtual async Task ProcessBlockAsync(ulong blockNumber)
        {
            if (_lastBlock != blockNumber)
            {
                ClearCacheOfProcessedTransactions();
                _lastBlock = blockNumber;
            }

            var block = await BlockProxy
                .Blocks
                .GetBlockWithTransactionsByNumber.SendRequestAsync(blockNumber.ToHexBigInteger())
                .ConfigureAwait(false);

            if(block == null)
                throw new BlockNotFoundException(blockNumber);

            if (await BlockFilters.IsMatchAsync(block))
            {
                await BlockHandler.HandleAsync(block);

                if (ProcessTransactionsInParallel)
                    await ProcessTransactionsMultiThreaded(block).ConfigureAwait(false);
                else
                    await ProcessTransactions(block).ConfigureAwait(false);
            }
        }

        public virtual async Task<ulong> GetMaxBlockNumberAsync()
        {
            var blockNumber = await BlockProxy.Blocks.GetBlockNumber.SendRequestAsync().ConfigureAwait(false);
            return blockNumber.ToUlong();
        }

        protected virtual void ClearCacheOfProcessedTransactions()
        {
            lock (_sync)
            {
                _processedTransactions.Clear();
            }
        }
       
        protected virtual bool HasAlreadyBeenProcessed(Transaction transaction)
        {
            lock (_sync)
            {
                return _processedTransactions.Contains(transaction.TransactionHash);
            }
        }

        protected virtual void MarkAsProcessed(Transaction transaction)
        {
            lock (_sync)
            {
                _processedTransactions.Add(transaction.TransactionHash);
            }
        }

        protected virtual async Task ProcessTransactions(BlockWithTransactions block)
        {
            foreach (var txn in block.Transactions)
            {
                if (!HasAlreadyBeenProcessed(txn))
                {
                    await TransactionProcessor.ProcessTransactionAsync(block, txn)
                        .ConfigureAwait(false);
                    MarkAsProcessed(txn);
                }
            }
        }

        protected virtual async Task ProcessTransactionsMultiThreaded(BlockWithTransactions block)
        {
            var txTasks = new List<Task>(block.Transactions.Length);

            foreach (var txn in block.Transactions)
            {
                if (!HasAlreadyBeenProcessed(txn))
                {
                    var task = TransactionProcessor.ProcessTransactionAsync(block, txn)
                        .ContinueWith((t) =>
                    {
                        MarkAsProcessed(txn);
                    });

                    txTasks.Add(task);
                }
            }

            await Task.WhenAll(txTasks).ConfigureAwait(false);
        }
    }
}