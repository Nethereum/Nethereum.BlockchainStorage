using Nethereum.BlockchainProcessing.Processing.Logs;
using System.Threading.Tasks;

namespace Nethereum.BlockchainProcessing.Processing.Logs.Configuration
{
    public interface ISubscriberContractsRepository
    {
        Task<ISubscriberContractDto> GetContractAsync(long subscriberId, long contractId);

        Task<ISubscriberContractDto> UpsertAsync(ISubscriberContractDto contract);
    }
}
