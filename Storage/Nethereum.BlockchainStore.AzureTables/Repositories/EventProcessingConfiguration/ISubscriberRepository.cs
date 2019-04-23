using Nethereum.BlockchainProcessing.Processing.Logs;
using System.Threading.Tasks;

namespace Nethereum.BlockchainStore.AzureTables.Repositories.EventProcessingConfiguration
{
    public interface ISubscriberRepository
    {
        Task<ISubscriberDto[]> GetSubscribersAsync(long partitionId);

        Task<ISubscriberDto> UpsertAsync(ISubscriberDto subscriber);
    }
}