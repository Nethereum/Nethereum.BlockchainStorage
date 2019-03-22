using System.Threading.Tasks;

namespace Nethereum.BlockchainProcessing.Processing.Logs.Handling
{
    public interface IDecodedEventHandler
    {
        Task<bool> HandleAsync(DecodedEvent decodedEvent);
    }
}
