using System.Threading.Tasks;

namespace Nethereum.BlockchainProcessing.Processing.Logs.Configuration
{
    public interface ISubscriberSearchIndexRepository
    {
        Task<ISubscriberSearchIndexDto> GetAsync(long subscriberId, long searchIndexId);
        Task<ISubscriberSearchIndexDto> UpsertAsync(ISubscriberSearchIndexDto dto);
    }
}
