using System.Threading.Tasks;

namespace Nethereum.BlockchainProcessing.Processing.Logs.Handling
{
    public interface IEventHandlerFactory
    {
        Task<IEventHandler> CreateAsync(EventHandlerDto config);
        Task SaveStateAsync();
    }
}
