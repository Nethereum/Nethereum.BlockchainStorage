using System.Threading.Tasks;

namespace Nethereum.BlockchainProcessing.Processing.Logs.Handling
{
    public interface ISubscriberQueueFactory
    {
        Task<ISubscriberQueue> GetSubscriberQueueAsync(long subscriberQueueId);
    }
}
