using Nethereum.BlockchainProcessing.Processing.Logs.Configuration;
using System.Threading.Tasks;

namespace Nethereum.BlockchainProcessing.Processing.Logs.Handling
{
    public interface ISubscriberSearchIndexConfigurationRepository
    { 
         Task<ISubscriberSearchIndexDto> GetSubscriberSearchIndexAsync(long subscriberId, long subscriberSearchIndexId);
    }
}
