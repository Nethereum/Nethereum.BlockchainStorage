using Nethereum.Contracts;
using System;
using System.Threading.Tasks;

namespace Nethereum.BlockchainStore.Search
{
    public interface IFunctionIndexer<TFunction> : IIndexer, IDisposable where TFunction : FunctionMessage, new()
    {
        Task IndexAsync(FunctionCall<TFunction> functionCall);
    }
}
