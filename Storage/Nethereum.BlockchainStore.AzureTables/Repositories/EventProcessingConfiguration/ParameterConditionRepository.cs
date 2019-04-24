using Microsoft.WindowsAzure.Storage.Table;
using Nethereum.BlockchainProcessing.Processing.Logs;
using Nethereum.BlockchainProcessing.Processing.Logs.Configuration;
using Nethereum.BlockchainStore.AzureTables.Entities.EventProcessingConfiguration;
using System.Threading.Tasks;

namespace Nethereum.BlockchainStore.AzureTables.Repositories.EventProcessingConfiguration
{
    public class ParameterConditionRepository : AzureTableRepository<ParameterConditionEntity>, IParameterConditionRepository
    {
        public ParameterConditionRepository(CloudTable table) : base(table)
        {
        }

        public static ParameterConditionEntity Map(IParameterConditionDto dto)
        {
            if (dto is ParameterConditionEntity entity) return entity;

            return new ParameterConditionEntity
            {
                Id = dto.Id,
                EventSubscriptionId = dto.EventSubscriptionId,
                Operator = dto.Operator,
                ParameterOrder = dto.ParameterOrder,
                Value = dto.Value
            };
        }

        public async Task<IParameterConditionDto[]> GetParameterConditionsAsync(long eventSubscriptionId)
        {
            return await GetManyAsync(eventSubscriptionId.ToString());
        }


        public async Task<IParameterConditionDto> UpsertAsync(IParameterConditionDto dto)
        {
            var entity = Map(dto);
            return await base.UpsertAsync(entity) as IParameterConditionDto;
        }
    }
}
