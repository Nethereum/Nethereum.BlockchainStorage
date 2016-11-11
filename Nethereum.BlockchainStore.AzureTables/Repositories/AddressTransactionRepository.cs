using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Table;
using Nethereum.BlockchainStore.Entities;
using Nethereum.Hex.HexTypes;
using Nethereum.RPC.Eth.DTOs;
using Wintellect.Azure.Storage.Table;
using Transaction = Nethereum.RPC.Eth.DTOs.Transaction;

namespace Nethereum.BlockchainStore.Repositories
{
    public class AddressTransactionRepository : IAddressTransactionRepository
    {
        protected AzureTable Table { get; set; }

        public AddressTransactionRepository(CloudTable cloudTable)
        {
            Table = new AzureTable(cloudTable);
        }

        public async Task UpsertAsync(Transaction transaction,
            TransactionReceipt transactionReceipt,
            bool failedCreatingContract,
            HexBigInteger blockTimestamp,
            string address,
            string error = null,
            bool hasVmStack = false,
            string newContractAddress = null)
        {
            var entity = AddressTransaction.CreateAddressTransaction(Table, transaction,
                transactionReceipt,
                failedCreatingContract, blockTimestamp, null, null, false, newContractAddress);
            await entity.InsertOrReplaceAsync().ConfigureAwait(false);
        }
    }
}