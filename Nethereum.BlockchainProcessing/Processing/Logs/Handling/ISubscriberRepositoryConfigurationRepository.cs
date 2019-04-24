using Nethereum.BlockchainProcessing.Processing.Logs.Configuration;
using System.Threading.Tasks;

namespace Nethereum.BlockchainProcessing.Processing.Logs.Handling
{
    public interface ISubscriberStorageConfigurationRepository
    {
        Task<ISubscriberStorageDto> GetSubscriberStorageAsync(long subscriberId, long subscriberRepositoryId);
    }
}
