using System.Threading.Tasks;
using Nethereum.BlockchainStore.Processors.Transactions;
using Nethereum.BlockchainStore.Web3Abstractions;

namespace Nethereum.BlockchainStore.Processors.PostProcessors
{
    public class BlockVmPostProcessor : BlockPostProcessor
    {
        public BlockVmPostProcessor(IGetBlockWithTransactionHashesByNumber blockProxy, IBlockRepository blockRepository, ITransactionProcessor transactionProcessor) : base(blockProxy, blockRepository, transactionProcessor)
        {
        }

        public override async Task ProcessBlockAsync(long blockNumber)
        {
            TransactionProcessor.EnabledValueProcessing = false;
            TransactionProcessor.EnabledContractCreationProcessing = false;
            TransactionProcessor.EnabledContractProcessing = true;
            TransactionProcessor.ContractTransactionProcessor.EnabledVmProcessing = true;
            await base.ProcessBlockAsync(blockNumber);
        }
    }
}