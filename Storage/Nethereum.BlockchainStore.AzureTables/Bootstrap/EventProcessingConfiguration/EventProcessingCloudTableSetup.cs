using System;
using Microsoft.WindowsAzure.Storage.Table;
using Nethereum.BlockchainProcessing.Processing.Logs.Configuration;
using Nethereum.BlockchainStore.AzureTables.Repositories.EventProcessingConfiguration;

namespace Nethereum.BlockchainStore.AzureTables.Bootstrap.EventProcessingConfiguration
{
    public class EventProcessingCloudTableSetup : CloudTableSetupBase
    {
        public EventProcessingCloudTableSetup(string connectionString, string prefix) : base(connectionString, prefix)
        {
        }

        public CloudTable GetSubscribersTable() => GetPrefixedTable("Subscribers");
        public CloudTable GetSubscriberContractsTable() => GetPrefixedTable("SubscriberContracts");

        public CloudTable GetEventSubscriptionsTable() => GetPrefixedTable("EventSubscriptions");

        public CloudTable GetEventSubscriptionAddressesTable() => GetPrefixedTable("EventSubscriptionAddresses");

        public CloudTable GetEventHandlerTable() => GetPrefixedTable("EventHandlers");

        public CloudTable GetParameterConditionTable() => GetPrefixedTable("ParameterConditions");

        public CloudTable GetEventSubscriptionStateTable() => GetPrefixedTable("EventSubscriptionStates");

        public CloudTable GetContractQueryTable() => GetPrefixedTable("ContractQueries");

        public CloudTable GetContractQueryParameterTable() => GetPrefixedTable("ContractQueryParameters");

        public CloudTable GetEventAggregatorTable() => GetPrefixedTable("EventAggregators");

        public ISubscriberRepository GetSubscriberRepository() => new SubscriberRepository(GetSubscribersTable());

        public ISubscriberContractsRepository GetSubscriberContractsRepository() => new SubscriberContractsRepository(GetSubscriberContractsTable());

        public IEventSubscriptionRepository GetEventSubscriptionsRepository() => new EventSubscriptionRepository(GetEventSubscriptionsTable());

        public IEventSubscriptionAddressRepository GetEventSubscriptionAddressesRepository() => new EventSubscriptionAddressRepository(GetEventSubscriptionAddressesTable());

        public IEventHandlerRepository GetEventHandlerRepository() => new EventHandlerRepository(GetEventHandlerTable());

        public IParameterConditionRepository GetParameterConditionRepository() => new ParameterConditionRepository(GetParameterConditionTable());

        public IEventSubscriptionStateRepository GetEventSubscriptionStateRepository() => new EventSubscriptionStateRepository(GetEventSubscriptionStateTable());

        public IContractQueryRepository GetContractQueryRepository() => new ContractQueryRepository(GetContractQueryTable());

        public IContractQueryParameterRepository GetContractQueryParameterRepository() => new ContractQueryParameterRepository(GetContractQueryParameterTable());

        public IEventAggregatorRepository GetEventAggregatorRepository() => new EventAggregatorRepository(GetEventAggregatorTable());
    }
}
