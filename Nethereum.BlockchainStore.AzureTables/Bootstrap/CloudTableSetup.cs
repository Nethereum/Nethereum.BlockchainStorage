using System.Net;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;

namespace Nethereum.BlockchainStore.Bootstrap
{
    public class CloudTableSetup
    {
        private readonly string connectionString;
        private CloudStorageAccount cloudStorageAccount;

        public CloudTableSetup(string connectionString)
        {
            this.connectionString = connectionString;
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

        public CloudTable GetTransactionsVmStackTable(string prefix = null)
        {
            return GetTable(prefix + "TransactionsVmStack");
        }

        public CloudTable GetTransactionsLogTable(string prefix = null)
        {
            return GetTable(prefix + "TransactionsLog");
        }

        public CloudTable GetTransactionsTable(string prefix = null)
        {
            return GetTable(prefix + "Transactions");
        }

        public CloudTable GetAddressTransactionsTable(string prefix = null)
        {
            return GetTable(prefix + "AddressTransactions");
        }

        public CloudTable GetBlocksTable(string prefix = null)
        {
            return GetTable(prefix + "Blocks");
        }

        public CloudTable GetContractsTable(string prefix = null)
        {
            return GetTable(prefix + "Contracts");
        }

        public CloudTable GetTable(string name)
        {
            var storageAccount = GetStorageAccount();
            var tableClient = storageAccount.CreateCloudTableClient();
            var table = tableClient.GetTableReference(name);
            table.CreateIfNotExists();
            return table;
        }
    }
}