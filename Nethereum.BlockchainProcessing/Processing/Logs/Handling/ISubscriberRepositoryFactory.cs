using Nethereum.BlockchainProcessing.Handlers;
using System.Threading.Tasks;

namespace Nethereum.BlockchainProcessing.Processing.Logs.Handling
{
    public interface ISubscriberStorageFactory
    {
        Task<ILogHandler> GetLogRepositoryAsync(long subscriberId, long subscriberReposistoryId);
    }
}
