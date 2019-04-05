using System.Threading.Tasks;

namespace Nethereum.BlockchainProcessing.Processing.Logs.Handling
{
    public interface IEventHandler
    {
        long Id { get;}
        Task<bool> HandleAsync(DecodedEvent decodedEvent);
    }
}
