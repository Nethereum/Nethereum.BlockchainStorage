using Nethereum.BlockchainProcessing.Processing.Logs;
using System.Threading.Tasks;

namespace Nethereum.BlockchainProcessing.Processing.Logs.Configuration
{
    public interface IEventHandlerRepository
    {
        Task<IEventHandlerDto[]> GetEventHandlersAsync(long eventSubscriptionId);
        Task<IEventHandlerDto> UpsertAsync(IEventHandlerDto dto);
    }

}
