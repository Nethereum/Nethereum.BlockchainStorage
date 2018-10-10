using Microsoft.WindowsAzure.Storage.Table;
using Nethereum.BlockchainStore.AzureTables.Entities;
using Nethereum.BlockchainStore.Repositories;
using Nethereum.Hex.HexTypes;
using Nethereum.RPC.Eth.DTOs;
using System.Threading.Tasks;
using Nethereum.BlockchainStore.Entities;
using Transaction = Nethereum.RPC.Eth.DTOs.Transaction;
using AddressTransaction = Nethereum.BlockchainStore.AzureTables.Entities.AddressTransaction;

namespace Nethereum.BlockchainStore.AzureTables.Repositories
{
    public class AddressTransactionRepository : AzureTableRepository<AddressTransaction>,  IAddressTransactionRepository
    {
        public AddressTransactionRepository(CloudTable cloudTable):base(cloudTable){}

        public async Task<ITransactionView> FindByAddressBlockNumberAndHashAsync(string address, HexBigInteger blockNumber, string transactionHash)
        {
            return await FindAddressTransactionAsync(address, blockNumber, transactionHash);
        }

        public async Task<IAddressTransactionView> FindAsync(string address, HexBigInteger blockNumber, string transactionHash)
        {
            return await FindAddressTransactionAsync(address, blockNumber, transactionHash);
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
                failedCreatingContract, blockTimestamp, address, error, hasVmStack, newContractAddress);

            await UpsertAsync(entity);
        }

        private async Task<AddressTransaction> FindAddressTransactionAsync(string address, HexBigInteger blockNumber, string transactionHash)
        {
            var partitionKey = address.ToPartitionKey();
            var rowKey = AddressTransaction.CreateRowKey(blockNumber, transactionHash);
            var operation = TableOperation.Retrieve<AddressTransaction>(partitionKey, rowKey);
            var results = await Table.ExecuteAsync(operation);
            return results.Result as AddressTransaction;
        }
    }
}