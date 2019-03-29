using Nethereum.BlockchainProcessing.Processing.Logs;
using System.Collections.Generic;

namespace Nethereum.BlockchainProcessing.Samples.SAS
{
    public class MockEventProcessingRepository
    {
        public List<SubscriberDto> Subscribers = new List<SubscriberDto>();
        public List<ContractDto> Contracts = new List<ContractDto>();
        public List<EventSubscriptionDto> EventSubscriptions = new List<EventSubscriptionDto>();
        public List<EventSubscriptionAddressDto> EventSubscriptionAddresses = new List<EventSubscriptionAddressDto>();
        public List<DecodedEventHandlerDto> DecodedEventHandlers = new List<DecodedEventHandlerDto>();
        public List<ContractQueryDto> ContractQueries = new List<ContractQueryDto>();
        public List<ContractQueryParameterDto> ContractQueryParameters = new List<ContractQueryParameterDto>();
        public List<ParameterConditionDto> ParameterConditions = new List<ParameterConditionDto>();
        public List<EventAggregatorConfigurationDto> EventAggregators = new List<EventAggregatorConfigurationDto>();
        public Dictionary<long, EventSubscriptionStateDto> EventSubscriptionStates = new Dictionary<long, EventSubscriptionStateDto>();

        public EventAggregatorConfigurationDto Add(EventAggregatorConfigurationDto dto)
        {
            EventAggregators.Add(dto);
            return dto;
        }

        public DecodedEventHandlerDto Add(DecodedEventHandlerDto dto)
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

        public ContractDto Add(ContractDto dto)
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
