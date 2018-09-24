using Microsoft.WindowsAzure.Storage.Table;
using Nethereum.BlockchainStore.AzureTables.Entities;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;
using Nethereum.BlockchainStore.Entities;
using Nethereum.BlockchainStore.Repositories;
using TransactionVmStack = Nethereum.BlockchainStore.AzureTables.Entities.TransactionVmStack;

namespace Nethereum.BlockchainStore.AzureTables.Repositories
{
    public class TransactionVMStackRepository : AzureTableRepository<TransactionVmStack>, ITransactionVMStackRepository
    {
        public TransactionVMStackRepository(CloudTable table) : base(table)
        {
        }

        public async Task<ITransactionVmStackView> FindByAddressAndTransactionHashAync(string address, string transactionHash)
        {
            var operation = TableOperation.Retrieve<Entities.TransactionVmStack>(address.ToPartitionKey(), transactionHash.ToRowKey());
            var results = await Table.ExecuteAsync(operation).ConfigureAwait(false);
            return results.Result as Entities.TransactionVmStack;
        }

        public Task<ITransactionVmStackView> FindByTransactionHashAync(string hash)
        {
            return Task.FromResult((ITransactionVmStackView)null);
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