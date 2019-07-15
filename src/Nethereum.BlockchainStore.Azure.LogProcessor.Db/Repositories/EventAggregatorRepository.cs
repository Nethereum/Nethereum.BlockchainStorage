using Microsoft.WindowsAzure.Storage.Table;
using Nethereum.BlockchainProcessing.Processing.Logs.Configuration;
using Nethereum.BlockchainStore.AzureTables.Entities.EventProcessingConfiguration;

namespace Nethereum.BlockchainStore.AzureTables.Repositories.EventProcessingConfiguration
{

    public class EventAggregatorRepository : 
        OneToOneRepository<IEventAggregatorDto, EventAggregatorEntity>, IEventAggregatorRepository
    {
        public EventAggregatorRepository(CloudTable table) : base(table)
        {
        }

        protected override EventAggregatorEntity Map(IEventAggregatorDto dto)
        {
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
    }
}
