using System.Threading.Tasks;
using Nethereum.BlockchainStore.Handlers;
using Nethereum.BlockchainStore.Processors.Transactions;
using Nethereum.BlockchainStore.Repositories;
using Nethereum.BlockchainStore.Web3Abstractions;

namespace Nethereum.BlockchainStore.Processors.PostProcessors
{
    public class BlockPostProcessor : BlockProcessor
    {
        public BlockPostProcessor(IGetBlockWithTransactionHashesByNumber blockProxy, IBlockHandler blockHandler, ITransactionProcessor transactionProcessor) : base(blockProxy, blockHandler, transactionProcessor)
        {
        }

        public override async Task ProcessBlockAsync(long blockNumber)
        {
            var block = await BlockProxy.GetBlockWithTransactionsHashesByNumber(blockNumber);
            //no need to save the block again
            await ProcessTransactions(block);
        }
    }
}