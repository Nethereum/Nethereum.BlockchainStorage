using Nethereum.BlockchainProcessing.Processing.Logs;
using Nethereum.BlockchainProcessing.Processing.Logs.Configuration;
using Nethereum.BlockchainProcessing.Processing.Logs.Handling.Handlers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Nethereum.BlockchainStore.AzureTables.Repositories.EventProcessingConfiguration
{
    public class AzureEventProcessingConfigurationRepository : IEventProcessingConfigurationRepository
    {
        public AzureEventProcessingConfigurationRepository(
            ISubscriberRepository subscriberRepository,
            ISubscriberContractsRepository subscriberContractRepository,
            IEventSubscriptionRepository eventSubscriptionRepository,
            IEventSubscriptionAddressRepository eventSubscriptionAddressRepository,
            IEventHandlerRepository eventHandlerRepository,
            IParameterConditionRepository parameterConditionRepository)
        {
            SubscriberRepository = subscriberRepository;
            SubscriberContractRepository = subscriberContractRepository;
            EventSubscriptionRepository = eventSubscriptionRepository;
            EventSubscriptionAddressRepository = eventSubscriptionAddressRepository;
            EventHandlerRepository = eventHandlerRepository;
            ParameterConditionRepository = parameterConditionRepository;
        }

        public ISubscriberRepository SubscriberRepository { get; }
        public ISubscriberContractsRepository SubscriberContractRepository { get; }
        public IEventSubscriptionRepository EventSubscriptionRepository { get; }
        public IEventSubscriptionAddressRepository EventSubscriptionAddressRepository { get; }
        public IEventHandlerRepository EventHandlerRepository { get; }
        public IParameterConditionRepository ParameterConditionRepository { get; }

        public Task<ISubscriberDto[]> GetSubscribersAsync(long partitionId) => SubscriberRepository.GetSubscribersAsync(partitionId);

        public Task<ISubscriberContractDto> GetSubscriberContractAsync(long subscriberId, long contractId) => SubscriberContractRepository.GetContractAsync(subscriberId, contractId);

        public Task<IEventSubscriptionDto[]> GetEventSubscriptionsAsync(long subscriberId) => EventSubscriptionRepository.GetEventSubscriptionsAsync(subscriberId);

        public Task<IEventSubscriptionAddressDto[]> GetEventSubscriptionAddressesAsync(long eventSubscriptionId) => EventSubscriptionAddressRepository.GetEventSubscriptionAddressesAsync(eventSubscriptionId);

        public Task<IEventHandlerDto[]> GetEventHandlersAsync(long eventSubscriptionId) => EventHandlerRepository.GetEventHandlersAsync(eventSubscriptionId);

        public Task<IParameterConditionDto[]> GetParameterConditionsAsync(long eventSubscriptionId) => ParameterConditionRepository.GetParameterConditionsAsync(eventSubscriptionId);

        public Task AddEventHandlerHistory(long eventHandlerId, string eventKey)
        {
            throw new NotImplementedException();
        }

        public Task<bool> ContainsEventHandlerHistory(long id, string eventKey)
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

        public Task<EventRuleConfiguration> GetEventRuleConfigurationAsync(long eventHandlerId)
        {
            throw new NotImplementedException();
        }


        public Task<IEventSubscriptionStateDto> GetOrCreateEventSubscriptionStateAsync(long eventSubscriptionId)
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

        public Task UpsertAsync(IEnumerable<IEventSubscriptionStateDto> state)
        {
            throw new NotImplementedException();
        }
    }
}
