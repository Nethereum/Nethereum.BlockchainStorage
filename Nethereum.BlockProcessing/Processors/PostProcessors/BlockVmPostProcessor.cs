using Nethereum.BlockchainProcessing.Handlers;
using Nethereum.BlockchainProcessing.Processors.Transactions;
using Nethereum.Web3;
using System.Threading.Tasks;

namespace Nethereum.BlockchainProcessing.Processors.PostProcessors
{
    public class BlockVmPostProcessor : BlockPostProcessor
    {
        public BlockVmPostProcessor(IWeb3 web3, IBlockHandler blockHandler, ITransactionProcessor transactionProcessor) : base(web3, blockHandler, transactionProcessor)
        {
        }

        public override Task ProcessBlockAsync(ulong blockNumber)
        {
            TransactionProcessor.EnabledValueProcessing = false;
            TransactionProcessor.EnabledContractCreationProcessing = false;
            TransactionProcessor.EnabledContractProcessing = true;
            TransactionProcessor.ContractTransactionProcessor.EnabledVmProcessing = true;
            return base.ProcessBlockAsync(blockNumber);
        }
    }
}