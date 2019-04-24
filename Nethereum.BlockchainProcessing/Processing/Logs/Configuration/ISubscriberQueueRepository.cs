using System.Threading.Tasks;

namespace Nethereum.BlockchainProcessing.Processing.Logs.Configuration
{
    public interface ISubscriberQueueRepository
    {
        Task<ISubscriberQueueDto> GetAsync(long subscriberId, long queueId);
        Task<ISubscriberQueueDto> UpsertAsync(ISubscriberQueueDto dto);
    }
}
