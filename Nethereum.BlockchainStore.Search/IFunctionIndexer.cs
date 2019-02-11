using Nethereum.Contracts;
using System.Collections.Generic;
using System.Threading.Tasks;
using Nethereum.BlockchainProcessing.Handlers;
using Nethereum.RPC.Eth.DTOs;

namespace Nethereum.BlockchainStore.Search
{
    public interface IFunctionIndexer<TFunctionMessage> where TFunctionMessage : FunctionMessage, new()
    {
        Task IndexAsync(FunctionCall<TFunctionMessage> functionMessage);
        Task IndexAsync(IEnumerable<FunctionCall<TFunctionMessage>> functionMessages);
        int Indexed { get; }
    }
}
