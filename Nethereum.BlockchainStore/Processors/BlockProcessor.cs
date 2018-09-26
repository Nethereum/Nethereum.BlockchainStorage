﻿using Nethereum.BlockchainStore.Processing;
using Nethereum.BlockchainStore.Processors;
using Nethereum.BlockchainStore.Processors.Transactions;
using Nethereum.Hex.HexTypes;
using Nethereum.RPC.Eth.DTOs;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nethereum.BlockchainStore.Repositories
{
    public class BlockProcessor : IBlockProcessor
    {
        private readonly IBlockRepository _blockRepository;
        private readonly List<IBlockFilter> _blockFilters;

        public static bool ProcessTransactionsInParallel { get; set; } = true;

        public BlockProcessor(Web3.Web3 web3, 
            IBlockRepository blockRepository,
            ITransactionProcessor transactionProcessor, 
            IEnumerable<IBlockFilter> blockFilters = null
           )
        {
            _blockRepository = blockRepository;
            TransactionProcessor = transactionProcessor;
            Web3 = web3;
            _blockFilters = new List<IBlockFilter>(blockFilters ?? new IBlockFilter[0]);
        }

        protected Web3.Web3 Web3 { get; set; }
        protected ITransactionProcessor TransactionProcessor { get; set; }

        public virtual async Task ProcessBlockAsync(long blockNumber)
        {
            var block = await GetBlockWithTransactionHashesAsync(blockNumber);

            if(block == null)
                throw new BlockNotFoundException(blockNumber);

            if (await _blockFilters.IgnoreAsync(block)) return;

            await _blockRepository.UpsertBlockAsync(block);

            if (ProcessTransactionsInParallel)
                await ProcessTransactionsMultiThreaded(block);
            else
                await ProcessTransactions(block);
        }

        protected async Task ProcessTransactions(BlockWithTransactionHashes block)
        {
            foreach (var txnHash in block.TransactionHashes)
            {
                await TransactionProcessor.ProcessTransactionAsync(txnHash, block);
            }
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

        protected async Task<BlockWithTransactionHashes> GetBlockWithTransactionHashesAsync(long blockNumber)
        {
            var block =
                await
                    Web3.Eth.Blocks.GetBlockWithTransactionsHashesByNumber.SendRequestAsync(
                        new HexBigInteger(blockNumber)).ConfigureAwait(false);
            return block;
        }
    }
}