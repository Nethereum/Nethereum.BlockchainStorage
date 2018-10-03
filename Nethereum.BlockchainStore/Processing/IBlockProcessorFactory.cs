using Nethereum.BlockchainStore.Processors;
using Nethereum.BlockchainStore.Processors.PostProcessors;
using Nethereum.BlockchainStore.Processors.Transactions;
using Nethereum.BlockchainStore.Repositories;
using Nethereum.BlockchainStore.Web3Abstractions;

namespace Nethereum.BlockchainStore.Processing
{
    public interface IBlockProcessorFactory
    {
        IBlockProcessor Create(IWeb3Wrapper web3, IVmStackErrorChecker vmStackErrorChecker, IBlockchainProcessingStrategy strategy, bool postVm = false);
    }

    public class BlockProcessorFactory : IBlockProcessorFactory
    {
        public IBlockProcessor Create(IWeb3Wrapper web3, IVmStackErrorChecker vmStackErrorChecker, IBlockchainProcessingStrategy strategy, bool postVm = false)
        {
            var contractTransactionProcessor = new ContractTransactionProcessor(
                web3, vmStackErrorChecker, strategy.ContractHandler,
                strategy.TransactionHandler, strategy.TransactionVmStackHandler, 
                strategy.TransactionLogHandler, strategy.Filters?.TransactionLogFilters);

            var contractCreationTransactionProcessor = new ContractCreationTransactionProcessor(
                web3, strategy.ContractHandler,
                strategy.TransactionHandler);

            var valueTransactionProcessor = new ValueTransactionProcessor(
                strategy.TransactionHandler);

            var transactionProcessor = new TransactionProcessor(
                web3, contractTransactionProcessor,
                valueTransactionProcessor, contractCreationTransactionProcessor, 
                strategy.Filters?.TransactionFilters, strategy.Filters?.TransactionReceiptFilters);

            if (postVm)
                return new BlockVmPostProcessor(
                    web3, strategy.BlockHandler, transactionProcessor);
            else
            {
                transactionProcessor.ContractTransactionProcessor.EnabledVmProcessing = false;
                return new BlockProcessor(
                    web3, strategy.BlockHandler, transactionProcessor, strategy.Filters?.BlockFilters);
            }  
        }
    }
}
