using Microsoft.WindowsAzure.Storage.Table;
using Nethereum.BlockchainProcessing.BlockStorage.Entities;
using Nethereum.BlockchainProcessing.BlockStorage.Repositories;
using Nethereum.BlockchainStore.AzureTables.Entities;
using Nethereum.Hex.HexTypes;
using Nethereum.RPC.Eth.DTOs;
using System.Threading.Tasks;
using AzureEntities = Nethereum.BlockchainStore.AzureTables.Entities;

namespace Nethereum.BlockchainStore.AzureTables.Repositories
{
    public class AddressTransactionRepository : 
        AzureTableRepository<AzureEntities.AddressTransaction>,  IAddressTransactionRepository
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

        private async Task<AzureEntities.AddressTransaction> FindAddressTransactionAsync(string address, HexBigInteger blockNumber, string transactionHash)
        {
            var partitionKey = address.ToPartitionKey();
            var rowKey = AzureEntities.AddressTransaction.CreateRowKey(blockNumber, transactionHash);
            var operation = TableOperation.Retrieve<AzureEntities.AddressTransaction>(partitionKey, rowKey);
            var results = await Table.ExecuteAsync(operation).ConfigureAwait(false);
            return results.Result as AzureEntities.AddressTransaction;
        }

        public async Task UpsertAsync(TransactionReceiptVO transactionReceiptVO, string address, string error = null, string newContractAddress = null)
        {
            var entity = AzureEntities.AddressTransaction.CreateAddressTransaction(transactionReceiptVO.Transaction,
                transactionReceiptVO.TransactionReceipt,
                transactionReceiptVO.HasError, transactionReceiptVO.BlockTimestamp, address, error, transactionReceiptVO.HasVmStack, newContractAddress);

            await UpsertAsync(entity).ConfigureAwait(false);
        }
    }
}