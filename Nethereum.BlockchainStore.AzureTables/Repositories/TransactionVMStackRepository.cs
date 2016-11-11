using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Table;
using Nethereum.BlockchainStore.Entities;
using Newtonsoft.Json.Linq;
using Wintellect.Azure.Storage.Table;

namespace Nethereum.BlockchainStore.Repositories
{
    public class TransactionVMStackRepository : ITransactionVMStackRepository
    {
        protected AzureTable Table { get; set; }

        public TransactionVMStackRepository(CloudTable cloudTable)
        {
            Table = new AzureTable(cloudTable);
        }

        public async Task UpsertAsync(string transactionHash,
            string address,
            JObject stackTrace)
        {
            var entity = TransactionVmStack.CreateTransactionVmStack(Table,
                transactionHash, address, stackTrace);
            await entity.InsertOrReplaceAsync();
        }
    }
}