using Microsoft.WindowsAzure.Storage.Table;
using Nethereum.BlockchainProcessing.Processing.Logs.Configuration;
using Nethereum.BlockchainStore.AzureTables.Entities.EventProcessingConfiguration;

namespace Nethereum.BlockchainStore.AzureTables.Repositories.EventProcessingConfiguration
{
    public class EventRuleRepository : OneToOneRepository<IEventRuleDto, EventRuleEntity>, IEventRuleRepository
    {
        public EventRuleRepository(CloudTable table) : base(table)
        {
        }

        protected override EventRuleEntity Map(IEventRuleDto dto)
        {
            return new EventRuleEntity
            {
                Id = dto.EventHandlerId,
                EventHandlerId = dto.EventHandlerId,
                EventParameterNumber = dto.EventParameterNumber,
                Source = dto.Source,
                InputName = dto.InputName,
                Type = dto.Type,
                Value = dto.Value
            };
        }
    }
}
