using Nethereum.BlockchainProcessing.Processing.Logs.Configuration;
using System.Threading.Tasks;

namespace Nethereum.BlockchainProcessing.Processing.Logs.Handling
{
    public interface IEventHandlerFactory
    {
        Task<IEventHandler> LoadAsync(IEventSubscription subscription, IEventHandlerDto config);
    }
}
