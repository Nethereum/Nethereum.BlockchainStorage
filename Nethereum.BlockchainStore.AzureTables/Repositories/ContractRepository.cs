using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Table;
using Nethereum.BlockchainStore.Entities;
using Wintellect.Azure.Storage.Table;
using Transaction = Nethereum.RPC.Eth.DTOs.Transaction;

namespace Nethereum.BlockchainStore.Repositories
{
    public class ContractRepository : IContractRepository
    {
        protected AzureTable Table { get; set; }

        public ContractRepository(CloudTable cloudTable)
        {
            Table = new AzureTable(cloudTable);
        }

        public async Task<bool> ExistsAsync(string contractAddress)
        {
            return await Contract.ExistsAsync(Table, contractAddress).ConfigureAwait(false);
        }

        public async Task UpsertAsync(string contractAddress, string code, Transaction transaction)
        {
            var contract = Contract.CreateContract(Table, contractAddress, code,
                transaction);
            await contract.InsertOrReplaceAsync().ConfigureAwait(false);
        }
    }
}