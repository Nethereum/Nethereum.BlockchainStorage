using Microsoft.WindowsAzure.Storage.Table;
using Nethereum.BlockchainStore.AzureTables.Entities;
using Nethereum.BlockchainStore.Repositories;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;

namespace Nethereum.BlockchainStore.AzureTables.Repositories
{
    public class TransactionVMStackRepository : AzureTableRepository<TransactionVmStack>, ITransactionVMStackRepository
    {
        public TransactionVMStackRepository(CloudTable table) : base(table)
        {
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