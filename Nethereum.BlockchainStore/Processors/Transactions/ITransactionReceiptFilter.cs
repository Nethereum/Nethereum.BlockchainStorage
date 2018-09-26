using Nethereum.RPC.Eth.DTOs;

namespace Nethereum.BlockchainStore.Processors.Transactions
{
    public interface ITransactionReceiptFilter
    {
        bool IsMatch(TransactionReceipt transactionReceipt);
    }
}