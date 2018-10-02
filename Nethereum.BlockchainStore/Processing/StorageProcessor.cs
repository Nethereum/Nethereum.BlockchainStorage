using Nethereum.BlockchainStore.Processors;
using Nethereum.BlockchainStore.Processors.PostProcessors;
using Nethereum.BlockchainStore.Processors.Transactions;
using Nethereum.BlockchainStore.Repositories;
using Nethereum.BlockchainStore.Web3Abstractions;
using NLog.Fluent;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Nethereum.BlockchainStore.Processing
{
    public class StorageProcessor: IDisposable
    {
        private const int MaxRetries = 3;
        private readonly IWeb3Wrapper _web3;
        private readonly IBlockProcessor _blockProcessor;
        private readonly IContractRepository _contractRepository;
        private readonly IBlockRepository _blockRepository;
        private readonly WaitForNextBlockStrategy _waitForNextBlockStrategy;
        private bool _contractCacheInitialised = false;
        private readonly List<object> _repositories = new List<object>();

        public long? MinimumBlockNumber { get; set; }

        public StorageProcessor(
            IWeb3Wrapper web3, 
            IBlockchainStoreRepositoryFactory repositoryFactory, 
            bool postVm = false,
            FilterContainer filterContainer = null
            )
        {
            _waitForNextBlockStrategy = new WaitForNextBlockStrategy();
            _web3 = web3;

            _blockRepository = repositoryFactory.CreateBlockRepository();
            _contractRepository = repositoryFactory.CreateContractRepository();

            var transactionRepository = repositoryFactory.CreateTransactionRepository();
            var addressTransactionRepository = repositoryFactory.CreateAddressTransactionRepository();
            var logRepository = repositoryFactory.CreateTransactionLogRepository();
            var vmStackRepository = repositoryFactory.CreateTransactionVmStackRepository();

            _repositories.Add(_blockRepository);
            _repositories.Add(transactionRepository);
            _repositories.Add(addressTransactionRepository);
            _repositories.Add(_contractRepository);
            _repositories.Add(logRepository);
            _repositories.Add(vmStackRepository);

            var vmStackErrorChecker = new VmStackErrorCheckerWrapper();

            var contractTransactionProcessor = new ContractTransactionProcessor(
                _web3, vmStackErrorChecker, _contractRepository,
                transactionRepository, addressTransactionRepository, vmStackRepository, 
                logRepository, filterContainer?.TransactionLogFilters);

            var contractCreationTransactionProcessor = new ContractCreationTransactionProcessor(
                _web3, _contractRepository,
                transactionRepository, addressTransactionRepository);

            var valueTransactionProcessor = new ValueTransactionProcessor(
                transactionRepository,
                addressTransactionRepository);

            var transactionProcessor = new TransactionProcessor(
                _web3, contractTransactionProcessor,
                valueTransactionProcessor, contractCreationTransactionProcessor, 
                filterContainer?.TransactionFilters, filterContainer?.TransactionReceiptFilters);

            if (postVm)
                _blockProcessor = new BlockVmPostProcessor(
                    _web3, _blockRepository, transactionProcessor);
            else
            {
                transactionProcessor.ContractTransactionProcessor.EnabledVmProcessing = false;
                _blockProcessor = new BlockProcessor(
                    _web3, _blockRepository, transactionProcessor, filterContainer?.BlockFilters);
            }       
        }

        private async Task InitContractCache()
        {
            if (!_contractCacheInitialised)
            {
                await _contractRepository.FillCache().ConfigureAwait(false);
                _contractCacheInitialised = true;
            }
        }

        public bool ProcessTransactionsInParallel { get; set; }

        /// <summary>
        /// Allow the processor to resume from where it left off
        /// </summary>
        private async Task<long> GetStartingBlockNumber()
        {
            var blockNumber = await _blockRepository.GetMaxBlockNumber();
            blockNumber = blockNumber <= 0 ? 0 : blockNumber - 1;

            if (MinimumBlockNumber.HasValue && MinimumBlockNumber > blockNumber)
                return MinimumBlockNumber.Value;

            return blockNumber;
        }

        public async Task<bool> ExecuteAsync(
            long? startBlock, long? endBlock, int retryNumber = 0)
        {
            _blockProcessor.ProcessTransactionsInParallel = ProcessTransactionsInParallel;

            startBlock = startBlock ?? await GetStartingBlockNumber();
            endBlock = endBlock ?? long.MaxValue;
            bool runContinuously = endBlock == long.MaxValue;
            
            await InitContractCache();

            while (startBlock <= endBlock)
                try
                {
                    System.Console.WriteLine(
                        $"{DateTime.Now.ToString("s")}. Block: {startBlock}. Attempt: {retryNumber}");

                    await _blockProcessor.ProcessBlockAsync(startBlock.Value).ConfigureAwait(false);
                    retryNumber = 0;
                    startBlock = startBlock + 1;
                }
                catch (BlockNotFoundException blockNotFoundException)
                {
                    System.Console.WriteLine(blockNotFoundException.Message);

                    if (runContinuously)
                    {
                        System.Console.WriteLine("Waiting for block...");
                        await _waitForNextBlockStrategy.Apply(retryNumber);
                        await ExecuteAsync(startBlock, endBlock, retryNumber + 1);
                    }
                    else
                    {
                        if (retryNumber != MaxRetries)
                        {
                            await ExecuteAsync(startBlock, endBlock, retryNumber + 1)
                                .ConfigureAwait(false);
                        }
                        else
                        {
                            retryNumber = 0;
                            startBlock = startBlock + 1;
                            Log.Error().Exception(blockNotFoundException)
                                .Message("BlockNumber" + startBlock).Write();
                            System.Console.WriteLine($"Skipping block");
                        }
                    }
                }
                catch (Exception ex)
                {
                    System.Console.WriteLine(
                        ex.Message + ". " + ex.InnerException?.Message);

                    if (retryNumber != MaxRetries)
                    {
                        await ExecuteAsync(startBlock, endBlock, retryNumber + 1)
                            .ConfigureAwait(false);
                    }
                    else
                    {
                        retryNumber = 0;
                        startBlock = startBlock + 1;
                        Log.Error().Exception(ex).Message(
                            "BlockNumber" + startBlock).Write();
                        System.Console.WriteLine(
                            "ERROR:" + startBlock + " " + DateTime.Now.ToString("s"));
                    }
                }

            return true;
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    foreach (var repo in _repositories)
                    {
                        if(repo is IDisposable disposableRepo)
                            disposableRepo.Dispose();
                    }

                    _repositories.Clear();
                }
                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~StorageProcessor() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion
    }
}