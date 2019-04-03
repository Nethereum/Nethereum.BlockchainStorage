using Nethereum.RPC.Eth.DTOs;

namespace Nethereum.BlockchainProcessing.Processors.Transactions
{
    public interface ITransactionReceiptFilter: IFilter<TransactionReceipt>
    {
    }
}