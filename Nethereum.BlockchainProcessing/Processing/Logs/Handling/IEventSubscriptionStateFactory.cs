using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Nethereum.BlockchainProcessing.Processing.Logs.Handling
{
    public interface IEventSubscriptionStateFactory
    {
        Task<EventSubscriptionStateDto> GetOrCreateEventSubscriptionState(long eventSubscriptionId);
        Task SaveAsync(IEnumerable<EventSubscriptionStateDto> state);
    }
}
