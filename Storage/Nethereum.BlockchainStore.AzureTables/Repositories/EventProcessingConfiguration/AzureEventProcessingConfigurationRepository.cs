using Nethereum.BlockchainProcessing.Processing.Logs;
using Nethereum.BlockchainProcessing.Processing.Logs.Configuration;
using Nethereum.BlockchainProcessing.Processing.Logs.Handling.Handlers;
using Nethereum.BlockchainStore.AzureTables.Bootstrap.EventProcessingConfiguration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nethereum.BlockchainStore.AzureTables.Repositories.EventProcessingConfiguration
{
    public class AzureEventProcessingConfigurationRepository : IEventProcessingConfigurationRepository
    {
        public AzureEventProcessingConfigurationRepository(EventProcessingCloudTableSetup cloudTableSetup)
            :this(
                 cloudTableSetup.GetSubscriberRepository(),
                cloudTableSetup.GetSubscriberContractsRepository(),
                cloudTableSetup.GetEventSubscriptionsRepository(),
                cloudTableSetup.GetEventSubscriptionAddressesRepository(),
                cloudTableSetup.GetEventHandlerRepository(),
                cloudTableSetup.GetParameterConditionRepository(),
                cloudTableSetup.GetEventSubscriptionStateRepository(),
                cloudTableSetup.GetContractQueryRepository(),
                cloudTableSetup.GetContractQueryParameterRepository(),
                cloudTableSetup.GetEventAggregatorRepository(),
                cloudTableSetup.GetSubscriberQueueRepository(),
                cloudTableSetup.GetSubscriberSearchIndexRepository(),
                cloudTableSetup.GetEventHandlerHistoryRepository(),
                cloudTableSetup.GetEventRuleRepository(),
                cloudTableSetup.GetSubscriberStorageRepository())
        {

        }

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
            IEventAggregatorRepository eventAggregatorRepository,
            ISubscriberQueueRepository subscriberQueueRepository,
            ISubscriberSearchIndexRepository subscriberSearchIndexRepository,
            IEventHandlerHistoryRepository eventHandlerHistoryRepository,
            IEventRuleRepository eventRuleRepository,
            ISubscriberStorageRepository subscriberStorageRepository
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
            SubscriberQueueRepository = subscriberQueueRepository;
            SubscriberSearchIndexRepository = subscriberSearchIndexRepository;
            EventHandlerHistoryRepository = eventHandlerHistoryRepository;
            EventRuleRepository = eventRuleRepository;
            SubscriberStorageRepository = subscriberStorageRepository;
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
        public ISubscriberQueueRepository SubscriberQueueRepository { get; }
        public ISubscriberSearchIndexRepository SubscriberSearchIndexRepository { get; }
        public IEventHandlerHistoryRepository EventHandlerHistoryRepository { get; }
        public IEventRuleRepository EventRuleRepository { get; }
        public ISubscriberStorageRepository SubscriberStorageRepository { get; }

        public Task<ISubscriberDto[]> GetSubscribersAsync(long partitionId) => SubscriberRepository.GetSubscribersAsync(partitionId);

        public Task<ISubscriberContractDto> GetSubscriberContractAsync(long subscriberId, long contractId) => SubscriberContractRepository.GetContractAsync(subscriberId, contractId);

        public Task<IEventSubscriptionDto[]> GetEventSubscriptionsAsync(long subscriberId) => EventSubscriptionRepository.GetEventSubscriptionsAsync(subscriberId);

        public Task<IEventSubscriptionAddressDto[]> GetEventSubscriptionAddressesAsync(long eventSubscriptionId) => EventSubscriptionAddressRepository.GetEventSubscriptionAddressesAsync(eventSubscriptionId);

        public Task<IEventHandlerDto[]> GetEventHandlersAsync(long eventSubscriptionId) => EventHandlerRepository.GetEventHandlersAsync(eventSubscriptionId);

        public Task<IParameterConditionDto[]> GetParameterConditionsAsync(long eventSubscriptionId) => ParameterConditionRepository.GetParameterConditionsAsync(eventSubscriptionId);

        public Task<IEventSubscriptionStateDto> GetOrCreateEventSubscriptionStateAsync(long eventSubscriptionId) => EventSubscriptionStateRepository.GetOrCreateEventSubscriptionStateAsync(eventSubscriptionId);

        public Task UpsertAsync(IEnumerable<IEventSubscriptionStateDto> state) => EventSubscriptionStateRepository.UpsertAsync(state);

        public Task<ISubscriberQueueDto> GetSubscriberQueueAsync(long subscriberId, long subscriberQueueId) => SubscriberQueueRepository.GetAsync(subscriberId, subscriberQueueId);

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

        public Task<ISubscriberSearchIndexDto> GetSubscriberSearchIndexAsync(long subscriberId, long subscriberSearchIndexId) => SubscriberSearchIndexRepository.GetAsync(subscriberId, subscriberSearchIndexId);

        public Task AddAsync(IEventHandlerHistoryDto dto) => EventHandlerHistoryRepository.AddAsync(dto);

        public Task<bool> ContainsEventHandlerHistoryAsync(long eventHandlerId, string eventKey) => EventHandlerHistoryRepository.ContainsAsync(eventHandlerId, eventKey);

        public Task<IEventRuleDto> GetEventRuleConfigurationAsync(long eventHandlerId) => EventRuleRepository.GetAsync(eventHandlerId);

        public Task<ISubscriberStorageDto> GetSubscriberStorageAsync(long subscriberId, long subscriberRepositoryId) => SubscriberStorageRepository.GetAsync(subscriberId, subscriberRepositoryId);

    }
}
