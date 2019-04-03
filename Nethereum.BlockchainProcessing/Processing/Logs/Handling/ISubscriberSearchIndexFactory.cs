using System.Threading.Tasks;

namespace Nethereum.BlockchainProcessing.Processing.Logs.Handling
{
    public interface ISubscriberSearchIndexFactory
    {
        Task<ISubscriberSearchIndex> GetSubscriberSearchIndexAsync(long subscriberSearchIndexId);
    }
}
