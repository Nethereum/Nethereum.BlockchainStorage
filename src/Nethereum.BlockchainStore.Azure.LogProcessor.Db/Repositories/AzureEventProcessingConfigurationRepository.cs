using Nethereum.BlockchainProcessing.Processing.Logs.Configuration;
using Nethereum.BlockchainStore.AzureTables.Bootstrap.EventProcessingConfiguration;

namespace Nethereum.BlockchainStore.AzureTables.Repositories.EventProcessingConfiguration
{
    public class AzureEventProcessingConfigurationRepository : EventProcessingConfigurationRepository
    {
        public AzureEventProcessingConfigurationRepository(EventProcessingCloudTableSetup cloudTableSetup)
            :base(
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
    }
}
