using System.Threading.Tasks;
using Nethereum.BlockchainStore.Processors;
using Nethereum.BlockchainStore.Processors.Transactions;
using Nethereum.Hex.HexTypes;
using Nethereum.RPC.Eth.DTOs;

namespace Nethereum.BlockchainStore.Repositories
{
    public class BlockProcessor : IBlockProcessor
    {
        private readonly IBlockRepository _blockRepository;

        public BlockProcessor(Web3.Web3 web3, 
            IBlockRepository blockRepository,
            ITransactionProcessor transactionProcessor
           )
        {
            _blockRepository = blockRepository;
            TransactionProcessor = transactionProcessor;
            Web3 = web3;
        }

        protected Web3.Web3 Web3 { get; set; }
        protected ITransactionProcessor TransactionProcessor { get; set; }

        public virtual async Task ProcessBlockAsync(long blockNumber)
        {
            var block = await GetBlockWithTransactionHashesAsync(blockNumber);

            await _blockRepository.UpsertBlockAsync(block);

            await ProcessTransactions(block);
        }

        protected async Task ProcessTransactions(BlockWithTransactionHashes block)
        {
            foreach (var txnHash in block.TransactionHashes)
            {
                await TransactionProcessor.ProcessTransactionAsync(txnHash, block).ConfigureAwait(false);
            }
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