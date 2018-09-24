using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Table;
using Nethereum.BlockchainStore.AzureTables.Entities;
using Nethereum.BlockchainStore.Entities;
using Nethereum.BlockchainStore.Repositories;
using Nethereum.Hex.HexTypes;
using Nethereum.RPC.Eth.DTOs;
using Transaction = Nethereum.RPC.Eth.DTOs.Transaction;

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

        public async Task UpsertAsync(string contractAddress, string code, Transaction transaction, TransactionReceipt transactionReceipt, bool failedCreatingContract, HexBigInteger blockTimestamp)
        {
            var transactionEntity = Entities.Transaction.CreateTransaction(
                transaction, transactionReceipt,
                failedCreatingContract, blockTimestamp, contractAddress);

            await UpsertAsync(transactionEntity).ConfigureAwait(false);
        }

        public async Task UpsertAsync(Transaction transaction,
          TransactionReceipt transactionReceipt,
            bool failed,
            HexBigInteger timeStamp, bool hasVmStack = false, string error = null)
        {
            var transactionEntity = Entities.Transaction.CreateTransaction(transaction,
                transactionReceipt,
                failed, timeStamp, hasVmStack, error);

            await UpsertAsync(transactionEntity).ConfigureAwait(false);
        }
       
    }
}