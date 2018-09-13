using System.Net;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using Nethereum.BlockchainStore.AzureTables.Repositories;
using Nethereum.BlockchainStore.Processors;
using Nethereum.BlockchainStore.Repositories;

namespace Nethereum.BlockchainStore.AzureTables.Bootstrap
{
    public class CloudTableSetup: IBlockchainStoreAzureTablesRepositoryFactory
    {
        private readonly string prefix;
        private readonly string connectionString;
        private CloudStorageAccount cloudStorageAccount;

        public CloudTableSetup(string connectionString, string prefix)
        {
            this.connectionString = connectionString;
            this.prefix = prefix;
        }

        public CloudStorageAccount GetStorageAccount()
        {
            if (cloudStorageAccount == null)
            {
                cloudStorageAccount = CloudStorageAccount.Parse(connectionString);
                var tableServicePoint = ServicePointManager.FindServicePoint(cloudStorageAccount.TableEndpoint);
                tableServicePoint.UseNagleAlgorithm = false;
                tableServicePoint.ConnectionLimit = 1000;
                ServicePointManager.DefaultConnectionLimit = 48;
                ServicePointManager.Expect100Continue = false;
                ServicePointManager.UseNagleAlgorithm = false;
            }
            return cloudStorageAccount;
        }

        public CloudTable GetTransactionsVmStackTable()
        {
            return GetTable(prefix + "TransactionsVmStack");
        }

        public CloudTable GetTransactionsLogTable()
        {
            return GetTable(prefix + "TransactionsLog");
        }

        public CloudTable GetTransactionsTable()
        {
            return GetTable(prefix + "Transactions");
        }

        public CloudTable GetAddressTransactionsTable()
        {
            return GetTable(prefix + "AddressTransactions");
        }

        public CloudTable GetBlocksTable()
        {
            return GetTable(prefix + "Blocks");
        }

        public CloudTable GetContractsTable()
        {
            return GetTable(prefix + "Contracts");
        }

        public CloudTable GetCountersTable()
        {
            return GetTable(prefix + "Counters");
        }

        public CloudTable GetTable(string name)
        {
            var storageAccount = GetStorageAccount();
            var tableClient = storageAccount.CreateCloudTableClient();
            var table = tableClient.GetTableReference(name);
            table.CreateIfNotExistsAsync().Wait();
            return table;
        }

        public IAddressTransactionRepository CreateAddressTransactionRepository() => CreateAzureTablesAddressTransactionRepository();
        public IBlockRepository CreateBlockRepository() => CreateAzureTablesBlockRepository();
        public IContractRepository CreateContractRepository() => CreateAzureTablesContractRepository();
        public ITransactionLogRepository CreateTransactionLogRepository() => CreateAzureTablesTransactionLogRepository();
        public ITransactionRepository CreateTransactionRepository() => CreateAzureTablesTransactionRepository();
        public ITransactionVMStackRepository CreateTransactionVmStackRepository() => CreateAzureTablesTransactionVmStackRepository();

        public IAzureTableAddressTransactionRepository CreateAzureTablesAddressTransactionRepository()
        {
            return new AddressTransactionRepository(GetTransactionsLogTable());
        }

        public IAzureTableBlockRepository CreateAzureTablesBlockRepository()
        {
            return new BlockRepository(GetBlocksTable(), GetCountersTable());
        }

        public IAzureTableContractRepository CreateAzureTablesContractRepository()
        {
            return new ContractRepository(GetContractsTable());
        }

        public IAzureTableTransactionLogRepository CreateAzureTablesTransactionLogRepository()
        {
            return new TransactionLogRepository(GetTransactionsLogTable());
        }

        public IAzureTableTransactionRepository CreateAzureTablesTransactionRepository()
        {
            return new TransactionRepository(GetTransactionsTable());
        }

        public IAzureTableTransactionVMStackRepository CreateAzureTablesTransactionVmStackRepository()
        {
            return new TransactionVMStackRepository(GetTransactionsVmStackTable());
        }

        public async Task DeleteAllTables()
        {
            var options = new TableRequestOptions { };
            var operationContext = new OperationContext() { };
            await GetCountersTable().DeleteIfExistsAsync(options, operationContext);
            await GetContractsTable().DeleteIfExistsAsync(options, operationContext);
            await GetAddressTransactionsTable().DeleteIfExistsAsync(options, operationContext);
            await GetBlocksTable().DeleteIfExistsAsync(options, operationContext);
            await GetTransactionsLogTable().DeleteIfExistsAsync(options, operationContext);
            await GetTransactionsTable().DeleteIfExistsAsync(options, operationContext);
            await GetTransactionsVmStackTable().DeleteIfExistsAsync(options, operationContext);
        }
    }
}