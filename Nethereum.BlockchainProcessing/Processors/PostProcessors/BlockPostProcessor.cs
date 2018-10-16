using System.Threading.Tasks;
using Nethereum.BlockchainProcessing.Handlers;
using Nethereum.BlockchainProcessing.Processors.Transactions;
using Nethereum.BlockchainProcessing.Web3Abstractions;

namespace Nethereum.BlockchainProcessing.Processors.PostProcessors
{
    public class BlockPostProcessor : BlockProcessor
    {
        public BlockPostProcessor(IBlockProxy blockProxy, IBlockHandler blockHandler, ITransactionProcessor transactionProcessor) : base(blockProxy, blockHandler, transactionProcessor)
        {
        }

        public override async Task ProcessBlockAsync(long blockNumber)
        {
            var block = await BlockProxy.GetBlockWithTransactionsAsync(blockNumber);
            //no need to save the block again
            await ProcessTransactions(block);
        }
    }
}