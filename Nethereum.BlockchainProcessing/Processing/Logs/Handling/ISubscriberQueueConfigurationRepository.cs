using Nethereum.BlockchainProcessing.Processing.Logs.Configuration;
using System.Threading.Tasks;

namespace Nethereum.BlockchainProcessing.Processing.Logs.Handling
{
    public interface ISubscriberQueueConfigurationRepository
    { 
         Task<SubscriberQueueConfigurationDto> GetSubscriberQueueAsync(long subscriberQueueId);
    }
}
