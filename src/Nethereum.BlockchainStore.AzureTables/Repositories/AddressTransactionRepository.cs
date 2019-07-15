using Microsoft.WindowsAzure.Storage.Table;
using Nethereum.BlockchainProcessing.Storage.Entities;
using Nethereum.BlockchainProcessing.Storage.Repositories;
using Nethereum.BlockchainStore.AzureTables.Entities;
using Nethereum.Hex.HexTypes;
using Nethereum.RPC.Eth.DTOs;
using System.Threading.Tasks;
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

        private async Task<AddressTransaction> FindAddressTransactionAsync(string address, HexBigInteger blockNumber, string transactionHash)
        {
            var partitionKey = address.ToPartitionKey();
            var rowKey = AddressTransaction.CreateRowKey(blockNumber, transactionHash);
            var operation = TableOperation.Retrieve<AddressTransaction>(partitionKey, rowKey);
            var results = await Table.ExecuteAsync(operation).ConfigureAwait(false);
            return results.Result as AddressTransaction;
        }

        public async Task UpsertAsync(TransactionReceiptVO transactionReceiptVO, string address, string error = null, string newContractAddress = null)
        {
            var entity = AddressTransaction.CreateAddressTransaction(transactionReceiptVO.Transaction,
                transactionReceiptVO.TransactionReceipt,
                transactionReceiptVO.HasError, transactionReceiptVO.BlockTimestamp, address, error, transactionReceiptVO.HasVmStack, newContractAddress);

            await UpsertAsync(entity).ConfigureAwait(false);
        }
    }
}