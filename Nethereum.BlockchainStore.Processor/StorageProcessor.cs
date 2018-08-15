using System;
using System.Threading;
using System.Threading.Tasks;
using Nethereum.BlockchainStore.EFCore;
using Nethereum.BlockchainStore.EFCore.Repositories;
using Nethereum.BlockchainStore.Processors;
using Nethereum.BlockchainStore.Processors.PostProcessors;
using Nethereum.BlockchainStore.Processors.Transactions;
using Nethereum.BlockchainStore.Repositories;
using NLog.Fluent;

namespace Nethereum.BlockchainStore.Processor
{
    public class StorageProcessor
    {
        private const int MaxRetries = 3;
        private readonly Web3.Web3 _web3;
        private readonly IBlockProcessor _procesor;
        private int _retryNumber;
        private readonly ContractRepository _contractRepository;

        public StorageProcessor(string url, IBlockchainDbContextFactory contextFactory, bool postVm = false)
        {
            _web3 = new Web3.Web3(url);

            var blockRepository = new BlockRepository(contextFactory);
            var transactionRepository = new TransactionRepository(contextFactory);
            var addressTransactionRepository = new AddressTransactionRepository(contextFactory);
            _contractRepository = new ContractRepository(contextFactory);
            var logRepository = new TransactionLogRepository(contextFactory);
            var vmStackRepository = new TransactionVMStackRepository(contextFactory);

            var contractTransactionProcessor = new ContractTransactionProcessor(_web3, _contractRepository,
                transactionRepository, addressTransactionRepository, vmStackRepository, logRepository);
            var contractCreationTransactionProcessor = new ContractCreationTransactionProcessor(_web3, _contractRepository,
                transactionRepository, addressTransactionRepository);
            var valueTrasactionProcessor = new ValueTransactionProcessor(transactionRepository,
                addressTransactionRepository);
            var transactionProcessor = new TransactionProcessor(_web3, contractTransactionProcessor,
                valueTrasactionProcessor, contractCreationTransactionProcessor);


            if (postVm)
                _procesor = new BlockVmPostProcessor(_web3, blockRepository, transactionProcessor);
            else
            {
                transactionProcessor.ContractTransactionProcessor.EnabledVmProcessing = false;
                _procesor = new BlockProcessor(_web3, blockRepository, transactionProcessor);
            }       
        }

        public async Task Init()
        {
            await _contractRepository.FillCache().ConfigureAwait(false);
        }

        public bool ProcessTransactionsInParallel
        {
            get => BlockProcessor.ProcessTransactionsInParallel;
            set => BlockProcessor.ProcessTransactionsInParallel = value;
        }

        public async Task<bool> ExecuteAsync(long startBlock, long endBlock)
        {
            await Init();
            while (startBlock <= endBlock)
                try
                {
                    await _procesor.ProcessBlockAsync(startBlock).ConfigureAwait(false);
                    _retryNumber = 0;
                    if (startBlock.ToString().EndsWith("0"))
                        System.Console.WriteLine(startBlock + " " + DateTime.Now.ToString("s"));

                    startBlock = startBlock + 1;
                }
                catch (Exception ex)
                {
                    System.Console.WriteLine(ex.Message);

                    if (ex.StackTrace.Contains("Only one usage of each socket address"))
                    {
                        Thread.Sleep(1000);
                        System.Console.WriteLine("SOCKET ERROR:" + startBlock + " " + DateTime.Now.ToString("s"));
                        await ExecuteAsync(startBlock, endBlock).ConfigureAwait(false);
                    }
                    else
                    {
                        if (_retryNumber != MaxRetries)
                        {
                            _retryNumber = _retryNumber + 1;
                            await ExecuteAsync(startBlock, endBlock).ConfigureAwait(false);
                        }
                        else
                        {
                            startBlock = startBlock + 1;
                            Log.Error().Exception(ex).Message("BlockNumber" + startBlock).Write();
                            System.Console.WriteLine("ERROR:" + startBlock + " " + DateTime.Now.ToString("s"));
                        }
                    }
                }

            return true;
        }
    }
}