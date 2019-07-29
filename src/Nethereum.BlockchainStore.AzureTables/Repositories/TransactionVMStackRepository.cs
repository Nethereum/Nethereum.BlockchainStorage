using Microsoft.WindowsAzure.Storage.Table;
using Nethereum.BlockchainProcessing.BlockStorage.Entities;
using Nethereum.BlockchainProcessing.BlockStorage.Repositories;
using Nethereum.BlockchainStore.AzureTables.Entities;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;
using TransactionVmStack = Nethereum.BlockchainStore.AzureTables.Entities.TransactionVmStack;

namespace Nethereum.BlockchainStore.AzureTables.Repositories
{
    public class TransactionVMStackRepository : 
        AzureTableRepository<TransactionVmStack>, ITransactionVMStackRepository
    {
        public TransactionVMStackRepository(CloudTable table) : base(table)
        {
        }

        public async Task<ITransactionVmStackView> FindByAddressAndTransactionHashAync(string address, string transactionHash)
        {
            var operation = TableOperation.Retrieve<TransactionVmStack>(address.ToPartitionKey(), transactionHash.ToRowKey());
            var results = await Table.ExecuteAsync(operation).ConfigureAwait(false);
            return results.Result as TransactionVmStack;
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