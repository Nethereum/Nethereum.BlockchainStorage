using System.Net;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using Nethereum.BlockchainStore.Processors;
using Nethereum.BlockchainStore.Repositories;

namespace Nethereum.BlockchainStore.Bootstrap
{
    public class CloudTableSetup: IBlockchainStoreRepositoryFactory
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

        public CloudTable GetTable(string name)
        {
            var storageAccount = GetStorageAccount();
            var tableClient = storageAccount.CreateCloudTableClient();
            var table = tableClient.GetTableReference(name);
            table.CreateIfNotExistsAsync().Wait();
            return table;
        }

        public IAddressTransactionRepository CreateAddressTransactionRepository()
        {
            return new AddressTransactionRepository(GetAddressTransactionsTable());
        }

        public IBlockRepository CreateBlockRepository()
        {
            return new BlockRepository(GetBlocksTable());
        }

        public IContractRepository CreateContractRepository()
        {
            return new ContractRepository(GetContractsTable());
        }

        public ITransactionLogRepository CreateTransactionLogRepository()
        {
            return new TransactionLogRepository(GetTransactionsLogTable());
        }

        public ITransactionRepository CreateTransactionRepository()
        {
            return new TransactionRepository(GetTransactionsTable());
        }

        public ITransactionVMStackRepository CreateTransactionVmStackRepository()
        {
            return new TransactionVMStackRepository(GetTransactionsVmStackTable());
        }
    }
}