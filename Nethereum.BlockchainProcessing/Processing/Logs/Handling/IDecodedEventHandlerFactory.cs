using System.Threading.Tasks;

namespace Nethereum.BlockchainProcessing.Processing.Logs.Handling
{
    public interface IDecodedEventHandlerFactory
    {
        Task<IDecodedEventHandler> CreateAsync(DecodedEventHandlerDto config);
        Task SaveStateAsync();
    }
}
