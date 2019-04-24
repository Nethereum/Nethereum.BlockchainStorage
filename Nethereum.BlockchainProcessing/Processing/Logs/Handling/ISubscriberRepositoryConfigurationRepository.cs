using Nethereum.BlockchainProcessing.Processing.Logs.Configuration;
using System.Threading.Tasks;

namespace Nethereum.BlockchainProcessing.Processing.Logs.Handling
{
    public interface ISubscriberRepositoryConfigurationRepository
    {
        Task<SubscriberRepositoryConfigurationDto> GetSubscriberRepositoryAsync(long subscriberRepositoryId);
    }
}
