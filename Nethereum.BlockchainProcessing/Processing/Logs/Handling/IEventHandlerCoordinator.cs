using Nethereum.ABI.Model;
using Nethereum.RPC.Eth.DTOs;
using System.Threading.Tasks;

namespace Nethereum.BlockchainProcessing.Processing.Logs.Handling
{
    public interface IEventHandlerCoordinator
    { 
        Task HandleAsync(EventABI eventAbi, params FilterLog[] eventLogs);
    }
}
