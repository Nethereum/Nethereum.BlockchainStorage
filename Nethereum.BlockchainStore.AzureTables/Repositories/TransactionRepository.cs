using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Table;
using Nethereum.Hex.HexTypes;
using Nethereum.RPC.Eth.DTOs;
using Wintellect.Azure.Storage.Table;
using Transaction = Nethereum.RPC.Eth.DTOs.Transaction;

namespace Nethereum.BlockchainStore.Repositories
{
    public class TransactionRepository : ITransactionRepository
    {
        protected AzureTable Table { get; set; }

        public TransactionRepository(CloudTable cloudTable)
        {
            Table = new AzureTable(cloudTable);
        }

        public async Task UpsertAsync(string contractAddress, string code, Transaction transaction, TransactionReceipt transactionReceipt, bool failedCreatingContract, HexBigInteger blockTimestamp)
        {
            var transactionEntity = Entities.Transaction.CreateTransaction(Table,
                transaction, transactionReceipt,
                failedCreatingContract, blockTimestamp, contractAddress);
            await transactionEntity.InsertOrReplaceAsync().ConfigureAwait(false);
        }

        public async Task UpsertAsync(Transaction transaction,
          TransactionReceipt transactionReceipt,
            bool failed,
            HexBigInteger timeStamp, bool hasVmStack = false, string error = null)
        {
            var transactionEntity = Entities.Transaction.CreateTransaction(Table, transaction,
                transactionReceipt,
                failed, timeStamp, hasVmStack, error);
            await transactionEntity.InsertOrReplaceAsync().ConfigureAwait(false);
        }
       
    }
}