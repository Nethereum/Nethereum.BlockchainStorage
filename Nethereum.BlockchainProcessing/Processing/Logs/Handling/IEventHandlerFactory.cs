using System.Threading.Tasks;

namespace Nethereum.BlockchainProcessing.Processing.Logs.Handling
{
    public interface IEventHandlerFactory
    {
        Task<IEventHandler> LoadAsync(EventHandlerDto config, EventSubscriptionStateDto state);
    }
}
