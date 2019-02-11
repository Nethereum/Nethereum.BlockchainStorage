using Nethereum.BlockchainProcessing.Handlers;
using Nethereum.Contracts;

namespace Nethereum.BlockchainStore.Search
{
    public struct FunctionCall<TDto> where TDto : FunctionMessage, new()
    {
        public FunctionCall(TransactionWithReceipt tx, TDto dto)
        {
            Tx = tx;
            Dto = dto;
        }

        public TransactionWithReceipt Tx { get; }
        public TDto Dto { get; }
    }
}
