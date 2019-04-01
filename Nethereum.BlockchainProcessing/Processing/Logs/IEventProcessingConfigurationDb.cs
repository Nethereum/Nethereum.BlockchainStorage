using Nethereum.BlockchainProcessing.Processing.Logs.Handling;
using System.Threading.Tasks;

namespace Nethereum.BlockchainProcessing.Processing.Logs
{
    public interface IEventProcessingConfigurationDb: 
        IEventSubscriptionStateFactory, 
        IEventContractQueryConfigurationFactory, 
        IEventAggregatorConfigurationFactory,
        ISubscriberQueueConfigurationFactory,
        ISubscriberSearchIndexConfigurationFactory
    {
        Task<ContractDto> GetContractAsync(long contractId);
        Task<EventSubscriptionAddressDto[]> GetEventAddressesAsync(long eventSubscriptionId);
        Task<EventSubscriptionDto[]> GetEventSubscriptionsAsync(long subscriberId);
        Task<ParameterConditionDto[]> GetParameterConditionsAsync(long eventSubscriptionId);
        Task<SubscriberDto[]> GetSubscribersAsync(long partitionId);
        Task<EventHandlerDto[]> GetDecodedEventHandlers(long eventSubscriptionId);

    }
}
