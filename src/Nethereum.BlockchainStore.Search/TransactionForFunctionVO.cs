using Nethereum.Contracts;
using Nethereum.RPC.Eth.DTOs;

namespace Nethereum.BlockchainStore.Search
{
    public class TransactionForFunctionVO<TDto> where TDto : FunctionMessage, new()
    {
        public TransactionForFunctionVO(TransactionReceiptVO tx, TDto dto)
        {
            Tx = tx;
            Dto = dto;
        }

        public TransactionReceiptVO Tx { get; }
        public TDto Dto { get; }
    }
}
