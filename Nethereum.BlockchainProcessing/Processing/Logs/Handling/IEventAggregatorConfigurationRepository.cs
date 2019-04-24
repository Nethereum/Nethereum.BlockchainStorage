using Nethereum.BlockchainProcessing.Processing.Logs.Handling.Handlers;
using System.Threading.Tasks;

namespace Nethereum.BlockchainProcessing.Processing.Logs.Handling
{
    public interface IEventAggregatorConfigurationRepository
    {
        Task<EventAggregatorConfiguration> GetEventAggregationConfigurationAsync(long eventHandlerId);
    }
}
