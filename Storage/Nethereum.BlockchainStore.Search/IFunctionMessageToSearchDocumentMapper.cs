using Nethereum.Contracts;

namespace Nethereum.BlockchainStore.Search
{
    public interface IFunctionMessageToSearchDocumentMapper<TFrom, out TSearchDocument> 
        where TFrom: FunctionMessage, new() where TSearchDocument : class, new()
    {
        TSearchDocument Map(FunctionCall<TFrom> from);
    }
}
