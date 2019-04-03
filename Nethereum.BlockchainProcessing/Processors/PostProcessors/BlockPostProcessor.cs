using System.Threading.Tasks;
using Nethereum.BlockchainProcessing.BlockchainProxy;
using Nethereum.BlockchainProcessing.Handlers;
using Nethereum.BlockchainProcessing.Processors.Transactions;

namespace Nethereum.BlockchainProcessing.Processors.PostProcessors
{
    public class BlockPostProcessor : BlockProcessor
    {
        public BlockPostProcessor(IBlockProxy blockProxy, IBlockHandler blockHandler, ITransactionProcessor transactionProcessor) : base(blockProxy, blockHandler, transactionProcessor)
        {
        }

        public override async Task ProcessBlockAsync(ulong blockNumber)
        {
            var block = await BlockProxy
                .GetBlockWithTransactionsAsync(blockNumber)
                .ConfigureAwait(false);
            //no need to save the block again
            await ProcessTransactions(block).ConfigureAwait(false);
        }
    }
}