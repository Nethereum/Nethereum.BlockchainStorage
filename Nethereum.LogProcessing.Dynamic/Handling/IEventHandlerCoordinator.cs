using Nethereum.ABI.Model;
using Nethereum.RPC.Eth.DTOs;
using System.Threading.Tasks;

namespace Nethereum.BlockchainProcessing.Processing.Logs.Handling
{
    public interface IEventHandlerManager
    { 
        Task HandleAsync(IEventSubscription subscription, EventABI[] eventAbis, params FilterLog[] eventLogs);

        Task HandleAsync<TEventDto>(IEventSubscription subscription, EventABI abi, params FilterLog[] eventLogs) where TEventDto : new();
    }
}
