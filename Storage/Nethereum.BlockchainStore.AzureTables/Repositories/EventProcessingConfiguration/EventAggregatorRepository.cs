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
}
