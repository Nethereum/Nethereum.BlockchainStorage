using Nethereum.BlockchainProcessing.Processors;
using Nethereum.BlockchainProcessing.Processors.PostProcessors;
using Nethereum.BlockchainProcessing.Processors.Transactions;
using Nethereum.BlockchainProcessing.Web3Abstractions;

namespace Nethereum.BlockchainProcessing.Processing
{
    public class BlockProcessorFactory : IBlockProcessorFactory
    {
        public IBlockProcessor Create(IWeb3Wrapper web3,
            IBlockchainProcessingStrategy strategy, bool postVm = false, bool processTransactionsInParallel = true)
        {
            return Create(web3, new VmStackErrorCheckerWrapper(), strategy, postVm, processTransactionsInParallel);
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