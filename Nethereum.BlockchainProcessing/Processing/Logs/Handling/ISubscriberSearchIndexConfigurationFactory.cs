using System.Threading.Tasks;

namespace Nethereum.BlockchainProcessing.Processing.Logs.Handling
{
    public interface ISubscriberSearchIndexConfigurationFactory
    { 
         Task<SubscriberSearchIndexConfigurationDto> GetSubscriberSearchIndexAsync(long subscriberSearchIndexId);
    }
}
