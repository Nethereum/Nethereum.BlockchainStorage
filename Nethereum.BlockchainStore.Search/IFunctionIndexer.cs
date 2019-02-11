using Nethereum.Contracts;
using System.Collections.Generic;
using System.Threading.Tasks;
using Nethereum.RPC.Eth.DTOs;

namespace Nethereum.BlockchainStore.Search
{
    public interface IFunctionIndexer<TFunctionMessage> where TFunctionMessage : FunctionMessage, new()
    {
        Task IndexAsync(Transaction tx, TFunctionMessage functionMessage);
        Task IndexAsync(IEnumerable<(Transaction tx, TFunctionMessage functionMessage)> transactions);
        int Indexed { get; }
    }
}
