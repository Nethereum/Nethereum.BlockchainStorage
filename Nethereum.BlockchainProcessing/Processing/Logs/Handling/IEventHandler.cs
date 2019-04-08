using System.Threading.Tasks;

namespace Nethereum.BlockchainProcessing.Processing.Logs.Handling
{
    public interface IEventHandler
    {
        IEventSubscription Subscription { get;}
        long Id { get;}
        Task<bool> HandleAsync(DecodedEvent decodedEvent);
    }
}
