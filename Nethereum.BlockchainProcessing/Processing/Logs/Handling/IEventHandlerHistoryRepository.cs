using Nethereum.BlockchainProcessing.Processing.Logs.Configuration;
using System.Threading.Tasks;

namespace Nethereum.BlockchainProcessing.Processing.Logs.Handling
{
    public interface IEventHandlerHistory
    {
        Task AddAsync(IEventHandlerHistoryDto dto);
        Task<bool> ContainsEventHandlerHistoryAsync(long eventHandlerId, string eventKey);
    }
}
