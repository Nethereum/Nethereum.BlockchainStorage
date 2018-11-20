using System.Threading.Tasks;
using Nethereum.BlockchainProcessing.Handlers;
using Nethereum.BlockchainProcessing.Processors.Transactions;
using Nethereum.BlockchainProcessing.Web3Abstractions;

namespace Nethereum.BlockchainProcessing.Processors.PostProcessors
{
    public class BlockVmPostProcessor : BlockPostProcessor
    {
        public BlockVmPostProcessor(IBlockProxy blockProxy, IBlockHandler blockHandler, ITransactionProcessor transactionProcessor) : base(blockProxy, blockHandler, transactionProcessor)
        {
        }

        public override async Task ProcessBlockAsync(ulong blockNumber)
        {
            TransactionProcessor.EnabledValueProcessing = false;
            TransactionProcessor.EnabledContractCreationProcessing = false;
            TransactionProcessor.EnabledContractProcessing = true;
            TransactionProcessor.ContractTransactionProcessor.EnabledVmProcessing = true;
            await base.ProcessBlockAsync(blockNumber);
        }
    }
}