using System.Threading.Tasks;

namespace Nethereum.BlockchainProcessing.Processing.Logs.Handling
{
    public interface ISubscriberRepositoryConfigurationFactory
    {
        Task<SubscriberRepositoryConfigurationDto> GetSubscriberRepositoryAsync(long subscriberRepositoryId);
    }
}
