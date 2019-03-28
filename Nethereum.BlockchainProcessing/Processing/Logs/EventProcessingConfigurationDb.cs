using System.Threading.Tasks;
using Nethereum.BlockchainProcessing.Processing.Logs.Handling;

namespace Nethereum.BlockchainProcessing.Processing.Logs
{
    public class EventProcessingConfigurationDb: IEventProcessingConfigurationDb
    {
        public Task<SubscriberDto[]> GetSubscribersAsync(long partitionId)
        {
            var subscribers = new SubscriberDto[]
            {
                new SubscriberDto
                {
                    PartitionId = partitionId,
                    Id = 0,
                    OrganisationName = ""
                }
            };

            return Task.FromResult(subscribers);
        }

        public Task<ContractDto> GetContractAsync(long contractId)
        {
            return Task.FromResult(
                new ContractDto
                {
                    Id = contractId,
                    Abi = "",
                    Name = ""
                });
        }

        public Task<EventSubscriptionDto[]> GetEventSubscriptionsAsync(long subscriberId)
        {
            var subscriptions = new EventSubscriptionDto[]
            {
                new EventSubscriptionDto
                {
                    ContractId = 0,
                    EventSignature = "",
                    Id = 0,
                    SubscriberId = 0
                }
            };

            return Task.FromResult(subscriptions);
        }

        public Task<EventSubscriptionAddressDto[]> GetEventAddressesAsync(long eventSubscriptionId)
        {
            var eventAddressDtos = new EventSubscriptionAddressDto[]
            {
                new EventSubscriptionAddressDto
                {
                    Address = "",
                    EventSubscriptionId = eventSubscriptionId,
                    Id = 0
                }
            };

            return Task.FromResult(eventAddressDtos);

        }

        public Task<ParameterConditionDto[]> GetParameterConditionsAsync(long eventSubscriptionId)
        {
            var conditions = new ParameterConditionDto[]
            {
                new ParameterConditionDto
                {
                    EventSubscriptionId = eventSubscriptionId,
                    Id = 0,
                    Operator = "=",
                    ParameterOrder = 0,
                    Value = ""
                }
            };
            return Task.FromResult(conditions);
        }

        public Task<DecodedEventHandlerDto[]> GetDecodedEventHandlers(long eventSubscriptionId)
        {
            throw new System.NotImplementedException();
        }

        public Task<EventSubscriptionStateDto> GetEventSubscriptionStateAsync(long eventSubscriptionId)
        {
            throw new System.NotImplementedException();
        }

        public Task SaveAsync(EventSubscriptionStateDto state)
        {
            throw new System.NotImplementedException();
        }

        public Task<ContractQueryConfiguration> GetContractQueryConfigurationAsync(long eventSubscriptionId)
        {
            throw new System.NotImplementedException();
        }

        public Task<EventAggregatorConfiguration> GetEventAggregationConfiguration(long eventSubscriptionId)
        {
            throw new System.NotImplementedException();
        }
    }
}
