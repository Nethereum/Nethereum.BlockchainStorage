using Microsoft.WindowsAzure.Storage.Table;
using Nethereum.BlockchainProcessing.Processing.Logs.Configuration;
using Nethereum.BlockchainStore.AzureTables.Entities.EventProcessingConfiguration;
using System.Threading.Tasks;

namespace Nethereum.BlockchainStore.AzureTables.Repositories.EventProcessingConfiguration
{

    public class EventAggregatorRepository : AzureTableRepository<EventAggregatorEntity>, IEventAggregatorRepository
    {
        public EventAggregatorRepository(CloudTable table) : base(table)
        {
        }

        public async Task<IEventAggregatorDto> GetEventAggregatorAsync(long eventHandlerId)
        {
            return await GetAsync(eventHandlerId.ToString(), eventHandlerId.ToString());
        }

        public static EventAggregatorEntity Map(IEventAggregatorDto dto)
        {
            if(dto is EventAggregatorEntity e) return e;

            return new EventAggregatorEntity
            {
                Id = dto.EventHandlerId,
                EventHandlerId = dto.EventHandlerId,
                Destination = dto.Destination,
                EventParameterNumber = dto.EventParameterNumber,
                Operation = dto.Operation,
                OutputKey = dto.OutputKey,
                Source = dto.Source,
                SourceKey = dto.SourceKey
            };
        }

        public async Task<IEventAggregatorDto> UpsertAsync(IEventAggregatorDto dto)
        {
            var entity = Map(dto);
            return await base.UpsertAsync(entity) as IEventAggregatorDto;
        }
    }

    public class EventRuleRepository : AzureTableRepository<EventRuleEntity>, IEventRuleRepository
    {
        public EventRuleRepository(CloudTable table) : base(table)
        {
        }

        public async Task<IEventRuleDto> GetAsync(long eventHandlerId)
        {
            return await GetAsync(eventHandlerId.ToString(), eventHandlerId.ToString());
        }

        public static EventRuleEntity Map(IEventRuleDto dto)
        {
            if (dto is EventRuleEntity e) return e;

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

        public async Task<IEventRuleDto> UpsertAsync(IEventRuleDto dto)
        {
            var entity = Map(dto);
            return await base.UpsertAsync(entity) as IEventRuleDto;
        }
    }
}
