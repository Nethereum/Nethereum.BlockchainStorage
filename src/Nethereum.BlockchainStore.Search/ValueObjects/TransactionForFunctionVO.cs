using Nethereum.Contracts;
using Nethereum.RPC.Eth.DTOs;

namespace Nethereum.BlockchainStore.Search
{
    public class TransactionForFunctionVO<TFunctionMessage> where TFunctionMessage : FunctionMessage, new()
    {
        public TransactionForFunctionVO(TransactionReceiptVO tx, TFunctionMessage functionMessage)
        {
            Transaction = tx;
            FunctionMessage = functionMessage;
        }

        public TransactionReceiptVO Transaction { get; }
        public TFunctionMessage FunctionMessage { get; }
    }
}
