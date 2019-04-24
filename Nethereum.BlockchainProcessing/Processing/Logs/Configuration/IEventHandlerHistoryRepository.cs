using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Nethereum.BlockchainProcessing.Processing.Logs.Configuration
{
    public interface IEventHandlerHistoryRepository
    {
        Task<IEventHandlerHistoryDto> AddAsync(IEventHandlerHistoryDto dto);

        Task<bool> ContainsAsync(long eventHandlerId, string eventKey);

        Task<IEventHandlerHistoryDto> GetAsync(long eventHandlerId, string eventKey);
        Task<IEventHandlerHistoryDto[]> GetAsync(long eventHandlerId);
    }
}
