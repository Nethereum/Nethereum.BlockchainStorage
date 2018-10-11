using Nethereum.BlockchainStore.Handlers;
using Nethereum.Hex.HexTypes;
using Nethereum.RPC.Eth.DTOs;
using System.Threading.Tasks;
using Transaction = Nethereum.RPC.Eth.DTOs.Transaction;

namespace Nethereum.BlockchainStore.Processors.Transactions
{
    public class ValueTransactionProcessor : IValueTransactionProcessor
    {
        private readonly ITransactionHandler _transactionHandler;

        public ValueTransactionProcessor(
            ITransactionHandler transactionHandler)
        {
            _transactionHandler = transactionHandler;
        }

        public async Task ProcessTransactionAsync(
            Transaction transaction, 
            TransactionReceipt transactionReceipt, 
            HexBigInteger blockTimestamp)
        {

            await _transactionHandler.HandleTransactionAsync(
                    new TransactionWithReceipt(transaction, transactionReceipt, false, blockTimestamp))
                .ConfigureAwait(false);
        }
    }
}