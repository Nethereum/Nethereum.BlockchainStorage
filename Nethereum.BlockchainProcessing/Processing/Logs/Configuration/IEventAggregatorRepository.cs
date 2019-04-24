using System.Threading.Tasks;

namespace Nethereum.BlockchainProcessing.Processing.Logs.Configuration
{
    public interface IEventAggregatorRepository
    {
        Task<IEventAggregatorDto> GetEventAggregatorAsync(long eventHandlerId);
        Task<IEventAggregatorDto> UpsertAsync(IEventAggregatorDto dto);
    }
}
