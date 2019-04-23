using System.Net;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;

namespace Nethereum.BlockchainStore.AzureTables.Bootstrap
{
    public abstract class CloudTableSetupBase
    {
        protected readonly string prefix;
        protected readonly string connectionString;
        protected CloudStorageAccount cloudStorageAccount;

        protected CloudTableSetupBase(string connectionString, string prefix)
        {
            this.connectionString = connectionString;
            this.prefix = prefix;
        }

        public virtual CloudStorageAccount GetStorageAccount()
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

        protected string PrefixTable(string table)
        {
            return prefix + table;
        }

        protected virtual CloudTable GetPrefixedTable(string name)
        {
            return GetTable(PrefixTable(name));
        }

        public virtual CloudTable GetTable(string name)
        {
            var storageAccount = GetStorageAccount();
            var tableClient = storageAccount.CreateCloudTableClient();
            var table = tableClient.GetTableReference(name);
            table.CreateIfNotExistsAsync().Wait();
            return table;
        }
    }
}