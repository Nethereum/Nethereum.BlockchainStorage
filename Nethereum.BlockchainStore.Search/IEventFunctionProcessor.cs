using System.Threading.Tasks;
using Nethereum.RPC.Eth.DTOs;

namespace Nethereum.BlockchainStore.Search
{
    public interface IEventFunctionProcessor  
    {
        Task Process(FilterLog[] logs);
    }
}
