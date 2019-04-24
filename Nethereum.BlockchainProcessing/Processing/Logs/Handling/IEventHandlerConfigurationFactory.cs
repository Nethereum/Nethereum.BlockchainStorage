using Nethereum.BlockchainProcessing.Processing.Logs.Configuration;
using System.Threading.Tasks;

namespace Nethereum.BlockchainProcessing.Processing.Logs.Handling
{
    public interface IEventHandlerConfigurationFactory
    {
        Task<EventSubscriptionStateDto> GetEventSubscriptionStateAsync(long eventSubscriptionId);
        Task SaveAsync(EventSubscriptionStateDto state);
    }
}
