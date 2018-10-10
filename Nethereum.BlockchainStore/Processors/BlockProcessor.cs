﻿using Nethereum.BlockchainStore.Processing;
using Nethereum.BlockchainStore.Processors;
using Nethereum.BlockchainStore.Processors.Transactions;
using Nethereum.BlockchainStore.Web3Abstractions;
using Nethereum.RPC.Eth.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;
using Nethereum.BlockchainStore.Handlers;

namespace Nethereum.BlockchainStore.Repositories
{
    public class BlockProcessor : IBlockProcessor
    {
        private readonly IBlockHandler _blockHandler;
        private readonly IEnumerable<IBlockFilter> _blockFilters;

        protected IBlockProxy BlockProxy { get; }
        protected ITransactionProcessor TransactionProcessor { get; set; }

        public bool ProcessTransactionsInParallel { get; set; } = true;

        public BlockProcessor(
            IBlockProxy blockProxy, 
            IBlockHandler blockHandler,
            ITransactionProcessor transactionProcessor, 
            IEnumerable<IBlockFilter> blockFilters = null
           )
        {
            BlockProxy = blockProxy;
            _blockHandler = blockHandler;
            TransactionProcessor = transactionProcessor;
            _blockFilters = blockFilters ?? new IBlockFilter[0];
        }

        public virtual async Task ProcessBlockAsync(long blockNumber)
        {
            var block = await BlockProxy.GetBlockWithTransactionsHashesByNumber(blockNumber);

            if(block == null)
                throw new BlockNotFoundException(blockNumber);

            if (await _blockFilters.IsMatchAsync(block))
            {
                await _blockHandler.HandleAsync(block);

                if (ProcessTransactionsInParallel)
                    await ProcessTransactionsMultiThreaded(block);
                else
                    await ProcessTransactions(block);
            }
        }

        protected async Task ProcessTransactions(BlockWithTransactionHashes block)
        {
            foreach (var txnHash in block.TransactionHashes)
                await TransactionProcessor.ProcessTransactionAsync(txnHash, block);
        }

        protected async Task ProcessTransactionsMultiThreaded(BlockWithTransactionHashes block)
        {
            var txTasks = new List<Task>(block.TransactionHashes.Length);
            foreach (var txnHash in block.TransactionHashes)
            {
                var task = TransactionProcessor.ProcessTransactionAsync(txnHash, block);
                txTasks.Add(task);
            }

            await Task.WhenAll(txTasks);
        }

        public async Task<long> GetMaxBlockNumberAsync()
        {
            return await BlockProxy.GetMaxBlockNumberAsync();
        }
    }
}