using Nethereum.Contracts;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Nethereum.BlockchainStore.Search
{
    public interface IFunctionIndexer<TFunctionMessage> : IIndexer where TFunctionMessage : FunctionMessage, new()
    {
        Task IndexAsync(FunctionCall<TFunctionMessage> functionMessage);
        Task IndexAsync(IEnumerable<FunctionCall<TFunctionMessage>> functionMessages);
    }
}
