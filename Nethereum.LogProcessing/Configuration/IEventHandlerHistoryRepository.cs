using System.Threading.Tasks;

namespace Nethereum.BlockchainProcessing.Processing.Logs.Configuration
{
    public interface IEventHandlerHistoryRepository
    {
        Task<IEventHandlerHistoryDto> UpsertAsync(IEventHandlerHistoryDto dto);

        Task<bool> ContainsAsync(long eventHandlerId, string eventKey);

        Task<IEventHandlerHistoryDto> GetAsync(long eventHandlerId, string eventKey);
        Task<IEventHandlerHistoryDto[]> GetManyAsync(long eventHandlerId);
    }
}
