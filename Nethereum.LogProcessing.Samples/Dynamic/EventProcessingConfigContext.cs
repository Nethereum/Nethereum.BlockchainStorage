using Nethereum.BlockchainProcessing.Processing.Logs;
using Nethereum.BlockchainProcessing.Processing.Logs.Configuration;
using System.Collections.Generic;
using System.Linq;

namespace Nethereum.LogProcessing.Samples.SAS
{
    public class EventProcessingConfigContext
    {
        public List<SubscriberDto> Subscribers = new List<SubscriberDto>();
        public List<SubscriberContractDto> Contracts = new List<SubscriberContractDto>();
        public List<EventSubscriptionDto> EventSubscriptions = new List<EventSubscriptionDto>();
        public List<EventSubscriptionAddressDto> EventSubscriptionAddresses = new List<EventSubscriptionAddressDto>();
        public List<EventHandlerDto> DecodedEventHandlers = new List<EventHandlerDto>();
        public List<ContractQueryDto> ContractQueries = new List<ContractQueryDto>();
        public List<ContractQueryParameterDto> ContractQueryParameters = new List<ContractQueryParameterDto>();
        public List<ParameterConditionDto> ParameterConditions = new List<ParameterConditionDto>();
        public List<EventAggregatorDto> EventAggregators = new List<EventAggregatorDto>();
        public List<IEventSubscriptionStateDto> EventSubscriptionStates = new List<IEventSubscriptionStateDto>();
        public List<SubscriberQueueDto> SubscriberQueues = new List<SubscriberQueueDto>();
        public List<SubscriberSearchIndexDto> SubscriberSearchIndexes = new List<SubscriberSearchIndexDto>();
        public List<IEventHandlerHistoryDto> EventHandlerHistories = new List<IEventHandlerHistoryDto>();
        public List<EventRuleConfigurationDto> EventRuleConfigurations = new List<EventRuleConfigurationDto>();
        public List<SubscriberStorageDto> SubscriberRepositories = new List<SubscriberStorageDto>();

        public IEventSubscriptionStateDto GetEventSubscriptionState(long eventSubscriptionId)
        {
            return EventSubscriptionStates.FirstOrDefault(s => s.EventSubscriptionId == eventSubscriptionId);
        }

        public SubscriberStorageDto Add(SubscriberStorageDto dto)
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

        public IEventHandlerHistoryDto Add(IEventHandlerHistoryDto dto)
        {
            EventHandlerHistories.Add(dto);
            return dto;
        }

        public SubscriberSearchIndexDto Add(SubscriberSearchIndexDto dto)
        {
            SubscriberSearchIndexes.Add(dto);
            return dto;
        }

        public SubscriberQueueDto Add(SubscriberQueueDto dto)
        {
            SubscriberQueues.Add(dto);
            return dto;
        }

        public EventAggregatorDto Add(EventAggregatorDto dto)
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
