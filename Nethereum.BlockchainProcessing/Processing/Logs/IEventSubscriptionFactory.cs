using System.Collections.Generic;
using System.Threading.Tasks;

namespace Nethereum.BlockchainProcessing.Processing.Logs
{
    public interface IEventSubscriptionFactory
    {
        Task<List<IEventSubscription>> LoadAsync(long partitionId);
    }
}
