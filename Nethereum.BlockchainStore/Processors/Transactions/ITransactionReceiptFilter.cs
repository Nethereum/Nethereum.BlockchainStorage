using Nethereum.RPC.Eth.DTOs;

namespace Nethereum.BlockchainStore.Processors.Transactions
{
    public interface ITransactionReceiptFilter: IFilter<TransactionReceipt>
    {
    }
}