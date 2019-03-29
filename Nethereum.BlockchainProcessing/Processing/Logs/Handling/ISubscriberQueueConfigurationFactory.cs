using System.Threading.Tasks;

namespace Nethereum.BlockchainProcessing.Processing.Logs.Handling
{
    public interface ISubscriberQueueConfigurationFactory
    { 
         Task<SubscriberQueueConfigurationDto> GetSubscriberQueueAsync(long subscriberQueueId);
    }
}
