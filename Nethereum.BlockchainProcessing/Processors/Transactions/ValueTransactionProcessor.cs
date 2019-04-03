using System.Threading.Tasks;
using Nethereum.BlockchainProcessing.Handlers;
using Nethereum.Hex.HexTypes;
using Nethereum.RPC.Eth.DTOs;
using Transaction = Nethereum.RPC.Eth.DTOs.Transaction;

namespace Nethereum.BlockchainProcessing.Processors.Transactions
{
    public class ValueTransactionProcessor : IValueTransactionProcessor
    {
        private readonly ITransactionHandler _transactionHandler;

        public ValueTransactionProcessor(
            ITransactionHandler transactionHandler)
        {
            _transactionHandler = transactionHandler;
        }

        public Task ProcessTransactionAsync(
            Transaction transaction, 
            TransactionReceipt transactionReceipt, 
            HexBigInteger blockTimestamp)
        {

            return _transactionHandler.HandleTransactionAsync(
                    new TransactionWithReceipt(
                        transaction, 
                        transactionReceipt, 
                        false, 
                        blockTimestamp));
        }
    }
}