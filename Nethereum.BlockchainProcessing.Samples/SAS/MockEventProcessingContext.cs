using Nethereum.BlockchainProcessing.Processing.Logs;
using System.Collections.Generic;
using System.Linq;

namespace Nethereum.BlockchainProcessing.Samples.SAS
{
    public class MockEventProcessingContext
    {
        public List<SubscriberDto> Subscribers = new List<SubscriberDto>();
        public List<SubscriberContractDto> Contracts = new List<SubscriberContractDto>();
        public List<EventSubscriptionDto> EventSubscriptions = new List<EventSubscriptionDto>();
        public List<EventSubscriptionAddressDto> EventSubscriptionAddresses = new List<EventSubscriptionAddressDto>();
        public List<EventHandlerDto> DecodedEventHandlers = new List<EventHandlerDto>();
        public List<ContractQueryDto> ContractQueries = new List<ContractQueryDto>();
        public List<ContractQueryParameterDto> ContractQueryParameters = new List<ContractQueryParameterDto>();
        public List<ParameterConditionDto> ParameterConditions = new List<ParameterConditionDto>();
        public List<EventAggregatorConfigurationDto> EventAggregators = new List<EventAggregatorConfigurationDto>();
        public List<EventSubscriptionStateDto> EventSubscriptionStates = new List<EventSubscriptionStateDto>();
        public List<SubscriberQueueConfigurationDto> SubscriberQueues = new List<SubscriberQueueConfigurationDto>();
        public List<SubscriberSearchIndexConfigurationDto> SubscriberSearchIndexes = new List<SubscriberSearchIndexConfigurationDto>();
        public List<EventHandlerHistoryDto> EventHandlerHistories = new List<EventHandlerHistoryDto>();
        public List<EventRuleConfigurationDto> EventRuleConfigurations = new List<EventRuleConfigurationDto>();
        public List<SubscriberRepositoryConfigurationDto> SubscriberRepositories = new List<SubscriberRepositoryConfigurationDto>();

        public EventSubscriptionStateDto GetEventSubscriptionState(long eventSubscriptionId)
        {
            return EventSubscriptionStates.FirstOrDefault(s => s.EventSubscriptionId == eventSubscriptionId);
        }

        public SubscriberRepositoryConfigurationDto Add(SubscriberRepositoryConfigurationDto dto)
        {
            SubscriberRepositories.Add(dto);
            return dto;
        }

        public EventRuleConfigurationDto Add(EventRuleConfigurationDto dto)
        {
            EventRuleConfigurations.Add(dto);
            return dto;
        }

        public EventSubscriptionStateDto Add(EventSubscriptionStateDto dto)
        {
            EventSubscriptionStates.Add(dto);
            return dto;
        }

        public EventHandlerHistoryDto Add(EventHandlerHistoryDto dto)
        {
            EventHandlerHistories.Add(dto);
            return dto;
        }

        public SubscriberSearchIndexConfigurationDto Add(SubscriberSearchIndexConfigurationDto dto)
        {
            SubscriberSearchIndexes.Add(dto);
            return dto;
        }

        public SubscriberQueueConfigurationDto Add(SubscriberQueueConfigurationDto dto)
        {
            SubscriberQueues.Add(dto);
            return dto;
        }

        public EventAggregatorConfigurationDto Add(EventAggregatorConfigurationDto dto)
        {
            EventAggregators.Add(dto);
            return dto;
        }

        public EventHandlerDto Add(EventHandlerDto dto)
        {
            DecodedEventHandlers.Add(dto);
            return dto;
        }

        public SubscriberDto Add(SubscriberDto dto)
        {
            Subscribers.Add(dto);
            return dto;
        }

        public EventSubscriptionDto Add(EventSubscriptionDto dto)
        {
            EventSubscriptions.Add(dto);
            return dto;
        }

        public SubscriberContractDto Add(SubscriberContractDto dto)
        {
            Contracts.Add(dto);
            return dto;
        }

        public ContractQueryDto Add(ContractQueryDto dto)
        {
            ContractQueries.Add(dto);
            return dto;
        }

        public ContractQueryParameterDto Add(ContractQueryParameterDto dto)
        {
            ContractQueryParameters.Add(dto);
            return dto;
        }

        public EventSubscriptionAddressDto Add(EventSubscriptionAddressDto dto)
        {
            EventSubscriptionAddresses.Add(dto);
            return dto;
        }
    }
}
