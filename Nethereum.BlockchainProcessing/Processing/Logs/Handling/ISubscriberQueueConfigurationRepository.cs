using Nethereum.BlockchainProcessing.Processing.Logs.Configuration;
using System.Threading.Tasks;

namespace Nethereum.BlockchainProcessing.Processing.Logs.Handling
{
    public interface ISubscriberQueueConfigurationRepository
    { 
         Task<ISubscriberQueueDto> GetSubscriberQueueAsync(long subscriberId, long subscriberQueueId);
    }
}
