using Nethereum.BlockchainProcessing.Processing.Logs.Configuration;
using System.Threading.Tasks;

namespace Nethereum.BlockchainProcessing.Processing.Logs.Matching
{
    public interface IEventMatcherFactory
    {
        Task<IEventMatcher> LoadAsync(IEventSubscriptionDto eventSubscription);
    }
}
