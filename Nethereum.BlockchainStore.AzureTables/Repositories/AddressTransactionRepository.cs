using Microsoft.WindowsAzure.Storage.Table;
using Nethereum.BlockchainStore.AzureTables.Entities;
using Nethereum.BlockchainStore.Repositories;
using Nethereum.Hex.HexTypes;
using Nethereum.RPC.Eth.DTOs;
using System.Threading.Tasks;
using Transaction = Nethereum.RPC.Eth.DTOs.Transaction;

namespace Nethereum.BlockchainStore.AzureTables.Repositories
{
    public class AddressTransactionRepository : AzureTableRepository<AddressTransaction>,  IAzureTableAddressTransactionRepository
    {
        public AddressTransactionRepository(CloudTable cloudTable):base(cloudTable){}

        public async Task<AddressTransaction> FindByBlockNumberAndHashAsync(HexBigInteger blockNumber, string transactionHash)
        {
            var operation = TableOperation.Retrieve<AddressTransaction>(blockNumber.Value.ToString(), transactionHash);
            var results = await Table.ExecuteAsync(operation);
            return results.Result as AddressTransaction;
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
            var entity = AddressTransaction.CreateAddressTransaction(transaction,
                transactionReceipt,
                failedCreatingContract, blockTimestamp, null, null, false, newContractAddress);

            var result = await UpsertAsync(entity);
        }
    }
}