using Nethereum.BlockchainProcessing.Processing.Logs;
using Nethereum.BlockchainProcessing.Processing.Logs.Configuration;
using Nethereum.BlockchainProcessing.Processing.Logs.Handling.Handlers;
using System;
using System.Collections.Generic;
using System.Linq;
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
            IParameterConditionRepository parameterConditionRepository,
            IEventSubscriptionStateRepository eventSubscriptionStateRepository,
            IContractQueryRepository contractQueryRepository,
            IContractQueryParameterRepository contractQueryParameterRepository,
            IEventAggregatorRepository eventAggregatorRepository
            )
        {
            SubscriberRepository = subscriberRepository;
            SubscriberContractRepository = subscriberContractRepository;
            EventSubscriptionRepository = eventSubscriptionRepository;
            EventSubscriptionAddressRepository = eventSubscriptionAddressRepository;
            EventHandlerRepository = eventHandlerRepository;
            ParameterConditionRepository = parameterConditionRepository;
            EventSubscriptionStateRepository = eventSubscriptionStateRepository;
            ContractQueryRepository = contractQueryRepository;
            ContractQueryParameterRepository = contractQueryParameterRepository;
            EventAggregatorRepository = eventAggregatorRepository;
        }

        public ISubscriberRepository SubscriberRepository { get; }
        public ISubscriberContractsRepository SubscriberContractRepository { get; }
        public IEventSubscriptionRepository EventSubscriptionRepository { get; }
        public IEventSubscriptionAddressRepository EventSubscriptionAddressRepository { get; }
        public IEventHandlerRepository EventHandlerRepository { get; }
        public IParameterConditionRepository ParameterConditionRepository { get; }
        public IEventSubscriptionStateRepository EventSubscriptionStateRepository { get; }
        public IContractQueryRepository ContractQueryRepository { get; }
        public IContractQueryParameterRepository ContractQueryParameterRepository { get; }
        public IEventAggregatorRepository EventAggregatorRepository { get; }

        public Task<ISubscriberDto[]> GetSubscribersAsync(long partitionId) => SubscriberRepository.GetSubscribersAsync(partitionId);

        public Task<ISubscriberContractDto> GetSubscriberContractAsync(long subscriberId, long contractId) => SubscriberContractRepository.GetContractAsync(subscriberId, contractId);

        public Task<IEventSubscriptionDto[]> GetEventSubscriptionsAsync(long subscriberId) => EventSubscriptionRepository.GetEventSubscriptionsAsync(subscriberId);

        public Task<IEventSubscriptionAddressDto[]> GetEventSubscriptionAddressesAsync(long eventSubscriptionId) => EventSubscriptionAddressRepository.GetEventSubscriptionAddressesAsync(eventSubscriptionId);

        public Task<IEventHandlerDto[]> GetEventHandlersAsync(long eventSubscriptionId) => EventHandlerRepository.GetEventHandlersAsync(eventSubscriptionId);

        public Task<IParameterConditionDto[]> GetParameterConditionsAsync(long eventSubscriptionId) => ParameterConditionRepository.GetParameterConditionsAsync(eventSubscriptionId);

        public Task<IEventSubscriptionStateDto> GetOrCreateEventSubscriptionStateAsync(long eventSubscriptionId) => EventSubscriptionStateRepository.GetOrCreateEventSubscriptionStateAsync(eventSubscriptionId);

        public Task UpsertAsync(IEnumerable<IEventSubscriptionStateDto> state) => EventSubscriptionStateRepository.UpsertAsync(state);

        public async Task<ContractQueryConfiguration> GetContractQueryConfigurationAsync(long subscriberId, long eventHandlerId)
        {
            return await ContractQueryRepository.LoadContractQueryConfiguration(
                subscriberId, eventHandlerId, SubscriberContractRepository, ContractQueryParameterRepository);
        }

        public async Task<EventAggregatorConfiguration> GetEventAggregationConfigurationAsync(long eventHandlerId)
        {
            var config = await EventAggregatorRepository.GetEventAggregatorAsync(eventHandlerId);
            return config.ToEventAggregatorConfiguration();
        }

        public Task AddEventHandlerHistory(long eventHandlerId, string eventKey)
        {
            throw new NotImplementedException();
        }

        public Task<bool> ContainsEventHandlerHistory(long id, string eventKey)
        {
            throw new NotImplementedException();
        }

        public Task<EventRuleConfiguration> GetEventRuleConfigurationAsync(long eventHandlerId)
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


    }
}
