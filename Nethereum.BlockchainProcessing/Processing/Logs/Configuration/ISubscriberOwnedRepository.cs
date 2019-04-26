using Nethereum.BlockchainProcessing.Processing.Logs;
using System.Threading.Tasks;

namespace Nethereum.BlockchainProcessing.Processing.Logs.Configuration
{
    public interface ISubscriberOwnedRepository<TDto> : IRepository<TDto>
    {
        Task<TDto> GetAsync(long subscriberId, long id);
        Task<TDto[]> GetManyAsync(long subscriberId);
    }
}
