using Nethereum.BlockchainProcessing.Processing.Logs;
using System.Threading.Tasks;

namespace Nethereum.BlockchainProcessing.Processing.Logs.Configuration
{
    public interface IParameterConditionRepository
    {
        Task<IParameterConditionDto[]> GetManyAsync(long eventSubscriptionId);
        Task<IParameterConditionDto> UpsertAsync(IParameterConditionDto dto);
    }
}
