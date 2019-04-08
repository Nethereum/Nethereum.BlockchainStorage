using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Nethereum.BlockchainProcessing.Processing.Logs.Handling
{
    public interface IEventSubscriptionStateFactory
    {
        Task<EventSubscriptionStateDto> GetOrCreateEventSubscriptionStateAsync(long eventSubscriptionId);
        Task UpsertAsync(IEnumerable<EventSubscriptionStateDto> state);
    }
}
