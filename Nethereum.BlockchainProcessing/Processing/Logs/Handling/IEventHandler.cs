using System.Threading.Tasks;

namespace Nethereum.BlockchainProcessing.Processing.Logs.Handling
{
    public interface IEventHandler
    {
        Task<bool> HandleAsync(DecodedEvent decodedEvent);
    }
}
