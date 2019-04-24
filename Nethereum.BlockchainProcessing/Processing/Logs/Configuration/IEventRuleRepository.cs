using System.Threading.Tasks;

namespace Nethereum.BlockchainProcessing.Processing.Logs.Configuration
{
    public interface IEventRuleRepository
    {
        Task<IEventRuleDto> GetAsync(long eventHandlerId);
        Task<IEventRuleDto> UpsertAsync(IEventRuleDto dto);
    }
}
