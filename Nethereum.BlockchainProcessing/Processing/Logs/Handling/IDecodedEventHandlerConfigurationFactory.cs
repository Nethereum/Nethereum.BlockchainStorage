using System.Threading.Tasks;

namespace Nethereum.BlockchainProcessing.Processing.Logs.Handling
{
    public interface IDecodedEventHandlerConfigurationFactory
    {
        Task<EventSubscriptionStateDto> GetEventSubscriptionStateAsync(long eventSubscriptionId);
        Task SaveAsync(EventSubscriptionStateDto state);
    }
}
