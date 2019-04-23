using Nethereum.BlockchainProcessing.Processing.Logs;
using Nethereum.BlockchainProcessing.Processing.Logs.Handling.Handlers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Nethereum.BlockchainStore.AzureTables.Repositories.EventProcessingConfiguration
{
    public class AzureEventProcessingConfigurationRepository : IEventProcessingConfigurationRepository
    {
        public AzureEventProcessingConfigurationRepository(ISubscriberRepository subscriberRepository)
        {
            SubscriberRepository = subscriberRepository;
        }

        public ISubscriberRepository SubscriberRepository { get; }

        public Task<ISubscriberDto[]> GetSubscribersAsync(long partitionId) => SubscriberRepository.GetSubscribersAsync(partitionId);


        public Task AddEventHandlerHistory(long eventHandlerId, string eventKey)
        {
            throw new NotImplementedException();
        }

        public Task<bool> ContainsEventHandlerHistory(long id, string eventKey)
        {
            throw new NotImplementedException();
        }

        public Task<SubscriberContractDto> GetContractAsync(long contractId)
        {
            throw new NotImplementedException();
        }

        public Task<ContractQueryConfiguration> GetContractQueryConfigurationAsync(long eventHandlerId)
        {
            throw new NotImplementedException();
        }

        public Task<EventAggregatorConfiguration> GetEventAggregationConfigurationAsync(long eventHandlerId)
        {
            throw new NotImplementedException();
        }

        public Task<EventHandlerDto[]> GetEventHandlers(long eventSubscriptionId)
        {
            throw new NotImplementedException();
        }

        public Task<EventRuleConfiguration> GetEventRuleConfigurationAsync(long eventHandlerId)
        {
            throw new NotImplementedException();
        }

        public Task<EventSubscriptionAddressDto[]> GetEventSubscriptionAddressesAsync(long eventSubscriptionId)
        {
            throw new NotImplementedException();
        }

        public Task<EventSubscriptionDto[]> GetEventSubscriptionsAsync(long subscriberId)
        {
            throw new NotImplementedException();
        }

        public Task<EventSubscriptionStateDto> GetOrCreateEventSubscriptionStateAsync(long eventSubscriptionId)
        {
            throw new NotImplementedException();
        }

        public Task<ParameterConditionDto[]> GetParameterConditionsAsync(long eventSubscriptionId)
        {
            throw new NotImplementedException();
        }

        public Task<SubscriberQueueConfigurationDto> GetSubscriberQueueAsync(long subscriberQueueId)
        {
            throw new NotImplementedException();
        }

        public Task<SubscriberRepositoryConfigurationDto> GetSubscriberRepositoryAsync(long subscriberRepositoryId)
        {
            throw new NotImplementedException();
        }

        public Task<SubscriberSearchIndexConfigurationDto> GetSubscriberSearchIndexAsync(long subscriberSearchIndexId)
        {
            throw new NotImplementedException();
        }

        public Task UpsertAsync(IEnumerable<EventSubscriptionStateDto> state)
        {
            throw new NotImplementedException();
        }
    }
}
