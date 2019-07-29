using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using Nethereum.BlockchainProcessing.BlockStorage.Repositories;
using Nethereum.BlockchainProcessing.ProgressRepositories;
using Nethereum.BlockchainStore.AzureTables.Repositories;
using System.Threading.Tasks;

namespace Nethereum.BlockchainStore.AzureTables.Bootstrap
{

    public class AzureTablesRepositoryFactory : 
        CloudTableSetupBase, IBlockchainStoreRepositoryFactory, IBlockProgressRepositoryFactory
    {
        public AzureTablesRepositoryFactory (string connectionString, string prefix):base(connectionString, prefix){ }

        public CloudTable GetCountersTable()
        {
            return GetPrefixedTable("Counters");
        }

        public CloudTable GetTransactionsVmStackTable()
        {
            return GetPrefixedTable("TransactionsVmStack");
        }

        public CloudTable GetTransactionsLogTable()
        {
            return GetPrefixedTable("TransactionLogs");
        }

        public CloudTable GetTransactionsTable()
        {
            return GetPrefixedTable("Transactions");
        }

        public CloudTable GetAddressTransactionsTable()
        {
            return GetPrefixedTable("AddressTransactions");
        }

        public CloudTable GetBlocksTable()
        {
            return GetPrefixedTable("Blocks");
        }

        public CloudTable GetContractsTable()
        {
            return GetPrefixedTable("Contracts");
        }

        public IBlockProgressRepository CreateBlockProgressRepository() => new BlockProgressRepository(GetCountersTable());
        public IAddressTransactionRepository CreateAddressTransactionRepository() => new AddressTransactionRepository(GetAddressTransactionsTable());
        public IBlockRepository CreateBlockRepository() => new BlockRepository(GetBlocksTable());
        public IContractRepository CreateContractRepository() => new ContractRepository(GetContractsTable());
        public ITransactionLogRepository CreateTransactionLogRepository() => new TransactionLogRepository(GetTransactionsLogTable());
        public ITransactionRepository CreateTransactionRepository() => new TransactionRepository(GetTransactionsTable());
        public ITransactionVMStackRepository CreateTransactionVmStackRepository() => new TransactionVMStackRepository(GetTransactionsVmStackTable());

        public async Task DeleteAllTables()
        {
            var options = new TableRequestOptions { };
            var operationContext = new OperationContext() { };
            await GetContractsTable().DeleteIfExistsAsync(options, operationContext);
            await GetAddressTransactionsTable().DeleteIfExistsAsync(options, operationContext);
            await GetBlocksTable().DeleteIfExistsAsync(options, operationContext);
            await GetTransactionsLogTable().DeleteIfExistsAsync(options, operationContext);
            await GetTransactionsTable().DeleteIfExistsAsync(options, operationContext);
            await GetTransactionsVmStackTable().DeleteIfExistsAsync(options, operationContext);
            await GetCountersTable().DeleteIfExistsAsync(options, operationContext);
        }
    }
}