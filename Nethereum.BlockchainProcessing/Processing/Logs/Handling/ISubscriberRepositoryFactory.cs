using Nethereum.BlockchainProcessing.Handlers;
using System.Threading.Tasks;

namespace Nethereum.BlockchainProcessing.Processing.Logs.Handling
{
    public interface ISubscriberRepositoryFactory
    {
        Task<ILogHandler> GetLogRepositoryAsync(long subscriberReposistoryId);
    }
}
