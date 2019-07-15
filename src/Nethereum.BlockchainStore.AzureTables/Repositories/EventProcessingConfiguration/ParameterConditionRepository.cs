using Microsoft.WindowsAzure.Storage.Table;
using Nethereum.BlockchainProcessing.Processing.Logs.Configuration;
using Nethereum.BlockchainStore.AzureTables.Entities.EventProcessingConfiguration;

namespace Nethereum.BlockchainStore.AzureTables.Repositories.EventProcessingConfiguration
{
    public class ParameterConditionRepository : OwnedRepository<IParameterConditionDto, ParameterConditionEntity>, IParameterConditionRepository
    {
        public ParameterConditionRepository(CloudTable table) : base(table)
        {
        }

        protected override ParameterConditionEntity Map(IParameterConditionDto dto)
        {
            return new ParameterConditionEntity
            {
                Id = dto.Id,
                EventSubscriptionId = dto.EventSubscriptionId,
                Operator = dto.Operator,
                ParameterOrder = dto.ParameterOrder,
                Value = dto.Value
            };
        }
    }
}
