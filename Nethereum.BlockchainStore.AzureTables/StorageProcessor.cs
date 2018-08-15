using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Table;
using Nethereum.BlockchainStore.Bootstrap;
using Nethereum.BlockchainStore.Entities;
using Nethereum.BlockchainStore.Processors;
using Nethereum.BlockchainStore.Processors.PostProcessors;
using Nethereum.BlockchainStore.Processors.Transactions;
using Nethereum.BlockchainStore.Repositories;
using NLog.Fluent;

namespace Nethereum.BlockchainStore.Processing.Console
{
    public class StorageProcessor
    {
        private const int MaxRetries = 3;
        private readonly CloudTable _contractTable;
        private readonly Web3.Web3 _web3;
        private readonly IBlockProcessor _procesor;
        private int _retryNumber;

        public StorageProcessor(string url, string connectionString, string prefix, bool postVm = false)
        {
            _web3 = new Web3.Web3(url);
            var tableSetup = new CloudTableSetup(connectionString);

            _contractTable = tableSetup.GetContractsTable(prefix);
            var transactionsTable = tableSetup.GetTransactionsTable(prefix);
            var addressTransactionsTable = tableSetup.GetAddressTransactionsTable(prefix);
            var blocksTable = tableSetup.GetBlocksTable(prefix);
            var logTable = tableSetup.GetTransactionsLogTable(prefix);
            var vmStackTable = tableSetup.GetTransactionsVmStackTable(prefix);

            var blockRepository = new BlockRepository(blocksTable);
            var transactionRepository = new TransactionRepository(transactionsTable);
            var addressTransactionRepository = new AddressTransactionRepository(addressTransactionsTable);
            var contractRepository = new ContractRepository(_contractTable);
            var logRepository = new TransactionLogRepository(logTable);
            var vmStackRepository = new TransactionVMStackRepository(vmStackTable);

            var contractTransactionProcessor = new ContractTransactionProcessor(_web3, contractRepository,
                transactionRepository, addressTransactionRepository, vmStackRepository, logRepository);
            var contractCreationTransactionProcessor = new ContractCreationTransactionProcessor(_web3, contractRepository,
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
            await Contract.InitContractsCacheAsync(_contractTable).ConfigureAwait(false);
        }

        public async Task<bool> ExecuteAsync(int startBlock, int endBlock)
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