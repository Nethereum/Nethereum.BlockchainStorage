using Nethereum.BlockchainProcessing.Processing.Logs.Handling;
using System.Threading.Tasks;

namespace Nethereum.BlockchainProcessing.Processing.Logs.Configuration
{
    public class EventHandlerHistory : IEventHandlerHistory
    {
        public EventHandlerHistory(IEventHandlerHistoryRepository repo)
        {
            Repo = repo;
        }

        public IEventHandlerHistoryRepository Repo { get; }

        public Task AddAsync(IEventHandlerHistoryDto dto) => Repo.UpsertAsync(dto);

        public Task<bool> ContainsEventHandlerHistoryAsync(long eventHandlerId, string eventKey) => Repo.ContainsAsync(eventHandlerId, eventKey);
    }
}
