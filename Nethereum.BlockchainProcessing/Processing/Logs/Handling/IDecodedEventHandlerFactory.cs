using System.Threading.Tasks;

namespace Nethereum.BlockchainProcessing.Processing.Logs.Handling
{
    public interface IDecodedEventHandlerFactory
    {
        Task<IDecodedEventHandler> CreateAsync(EventHandlerDto config);
        Task SaveStateAsync();
    }
}
