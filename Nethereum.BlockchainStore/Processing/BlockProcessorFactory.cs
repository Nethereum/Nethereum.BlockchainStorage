using Nethereum.BlockchainStore.Processors;
using Nethereum.BlockchainStore.Processors.PostProcessors;
using Nethereum.BlockchainStore.Processors.Transactions;
using Nethereum.BlockchainStore.Repositories;
using Nethereum.BlockchainStore.Web3Abstractions;

namespace Nethereum.BlockchainStore.Processing
{
    public class BlockProcessorFactory : IBlockProcessorFactory
    {
        public IBlockProcessor Create(IWeb3Wrapper web3,
            IBlockchainProcessingStrategy strategy, bool postVm = false, bool processTransactionsInParallel = true)
        {
            return Create(web3, new VmStackErrorCheckerWrapper(), strategy, postVm);
        }

        public IBlockProcessor Create(
            IWeb3Wrapper web3, IVmStackErrorChecker vmStackErrorChecker, 
            IBlockchainProcessingStrategy strategy, bool postVm = false, bool processTransactionsInParallel = true)
        {

            var transactionLogProcessor = new TransactionLogProcessor(
                strategy.Filters?.TransactionLogFilters,
                strategy.TransactionLogHandler);

            var contractTransactionProcessor = new ContractTransactionProcessor(
                web3, vmStackErrorChecker, strategy.ContractHandler,
                strategy.TransactionHandler, strategy.TransactionVmStackHandler);

            var contractCreationTransactionProcessor = new ContractCreationTransactionProcessor(
                web3, strategy.ContractHandler,
                strategy.TransactionHandler);

            var valueTransactionProcessor = new ValueTransactionProcessor(
                strategy.TransactionHandler);

            var transactionProcessor = new TransactionProcessor(
                web3, 
                contractTransactionProcessor,
                valueTransactionProcessor, 
                contractCreationTransactionProcessor, 
                transactionLogProcessor,
                strategy.Filters?.TransactionFilters, 
                strategy.Filters?.TransactionReceiptFilters,
                strategy.Filters?.TransactionAndReceiptFilters);

            if (postVm)
                return new BlockVmPostProcessor(
                    web3, strategy.BlockHandler, transactionProcessor)
                {
                    ProcessTransactionsInParallel = processTransactionsInParallel
                };

            transactionProcessor.ContractTransactionProcessor.EnabledVmProcessing = false;
            return new BlockProcessor(
                web3, strategy.BlockHandler, transactionProcessor, strategy.Filters?.BlockFilters)
            {
                ProcessTransactionsInParallel = processTransactionsInParallel
            };
        }
    }
}