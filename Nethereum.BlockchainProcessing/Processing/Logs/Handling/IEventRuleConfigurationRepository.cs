using Nethereum.BlockchainProcessing.Processing.Logs.Handling.Handlers;
using System.Threading.Tasks;

namespace Nethereum.BlockchainProcessing.Processing.Logs.Handling
{
    public interface IEventRuleConfigurationRepository
    {
        Task<EventRuleConfiguration> GetEventRuleConfigurationAsync(long eventHandlerId);
    }
}
