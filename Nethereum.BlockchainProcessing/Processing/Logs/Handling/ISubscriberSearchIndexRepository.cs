using System.Threading.Tasks;

namespace Nethereum.BlockchainProcessing.Processing.Logs.Handling
{
    public interface ISubscriberSearchIndexRepository
    {
        Task<ISubscriberSearchIndex> GetSubscriberSearchIndexAsync(long subscriberSearchIndexId);
    }
}
