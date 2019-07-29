using Microsoft.WindowsAzure.Storage.Table;
using Nethereum.BlockchainProcessing.BlockStorage.Entities;
using Nethereum.BlockchainProcessing.BlockStorage.Repositories;
using Nethereum.BlockchainStore.AzureTables.Entities;
using Nethereum.Hex.HexTypes;
using Nethereum.RPC.Eth.DTOs;
using System.Threading.Tasks;

namespace Nethereum.BlockchainStore.AzureTables.Repositories
{
    public class TransactionRepository : AzureTableRepository<Entities.Transaction>, ITransactionRepository
    {
        public TransactionRepository(CloudTable table) : base(table)
        {
        }

        public async Task<ITransactionView> FindByBlockNumberAndHashAsync(HexBigInteger blockNumber, string transactionHash)
        {
            var operation = TableOperation.Retrieve<Entities.Transaction>(blockNumber.Value.ToString().ToPartitionKey(), transactionHash.ToRowKey());
            var results = await Table.ExecuteAsync(operation).ConfigureAwait(false);
            return results.Result as Entities.Transaction;
        }

        public async Task UpsertAsync(TransactionReceiptVO transactionReceiptVO, string code, bool failedCreatingContract)
        {
            var transactionEntity = Entities.Transaction.CreateTransaction(
                transactionReceiptVO.Transaction, transactionReceiptVO.TransactionReceipt,
                failedCreatingContract, transactionReceiptVO.BlockTimestamp, transactionReceiptVO.TransactionReceipt.ContractAddress);

            await UpsertAsync(transactionEntity).ConfigureAwait(false);
        }

        public async Task UpsertAsync(TransactionReceiptVO transactionReceiptVO)
        {
            var transactionEntity = Entities.Transaction.CreateTransaction(transactionReceiptVO.Transaction,
                transactionReceiptVO.TransactionReceipt,
                transactionReceiptVO.HasError, transactionReceiptVO.BlockTimestamp, transactionReceiptVO.HasVmStack, transactionReceiptVO.Error);

            await UpsertAsync(transactionEntity).ConfigureAwait(false);
        }
    }
}