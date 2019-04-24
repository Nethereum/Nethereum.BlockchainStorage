using Nethereum.BlockchainProcessing.Processing.Logs;
using System.Threading.Tasks;

namespace Nethereum.BlockchainProcessing.Processing.Logs.Configuration
{
    public interface ISubscriberRepository
    {
        Task<ISubscriberDto[]> GetSubscribersAsync(long partitionId);

        Task<ISubscriberDto> UpsertAsync(ISubscriberDto subscriber);
    }
}