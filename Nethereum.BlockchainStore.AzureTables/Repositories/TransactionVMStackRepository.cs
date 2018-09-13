using Microsoft.WindowsAzure.Storage.Table;
using Nethereum.BlockchainStore.AzureTables.Entities;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;

namespace Nethereum.BlockchainStore.AzureTables.Repositories
{
    public class TransactionVMStackRepository : AzureTableRepository<TransactionVmStack>, IAzureTableTransactionVMStackRepository
    {
        public TransactionVMStackRepository(CloudTable table) : base(table)
        {
        }

        public async Task<TransactionVmStack> FindByAddressAndTransactionHashAync(string address, string transactionHash)
        {
            var operation = TableOperation.Retrieve<Entities.TransactionVmStack>(address.ToPartitionKey(), transactionHash.ToRowKey());
            var results = await Table.ExecuteAsync(operation).ConfigureAwait(false);
            return results.Result as Entities.TransactionVmStack;
        }

        public async Task UpsertAsync(string transactionHash,
            string address,
            JObject stackTrace)
        {
            var entity = TransactionVmStack.CreateTransactionVmStack(transactionHash, address, stackTrace);

            await UpsertAsync(entity).ConfigureAwait(false);
        }
    }
}