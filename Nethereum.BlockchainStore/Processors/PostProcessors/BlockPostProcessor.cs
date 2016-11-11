using System.Threading.Tasks;
using Nethereum.BlockchainStore.Processors.Transactions;
using Nethereum.BlockchainStore.Repositories;

namespace Nethereum.BlockchainStore.Processors.PostProcessors
{
    public class BlockPostProcessor : BlockProcessor
    {
        public BlockPostProcessor(Web3.Web3 web3, IBlockRepository blockRepository, ITransactionProcessor transactionProcessor) : base(web3, blockRepository, transactionProcessor)
        {
        }

        public override async Task ProcessBlockAsync(long blockNumber)
        {
            var block = await GetBlockWithTransactionHashesAsync(blockNumber);
            //no need to save the block again
            await ProcessTransactions(block);
        }
    }
}