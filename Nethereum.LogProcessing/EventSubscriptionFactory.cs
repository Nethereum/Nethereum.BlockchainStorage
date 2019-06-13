using Nethereum.BlockchainProcessing.Processing.Logs.Configuration;
using Nethereum.BlockchainProcessing.Processing.Logs.Handling;
using Nethereum.BlockchainProcessing.Processing.Logs.Matching;
using Nethereum.Web3;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nethereum.BlockchainProcessing.Processing.Logs
{
    public class EventSubscriptionFactory : IEventSubscriptionFactory
    {
        public IEventMatcherFactory EventMatcherFactory { get; }

        public IEventHandlerFactory EventHandlerFactory { get; }
        public IEventProcessingConfigurationRepository ConfigurationRepository { get; }

        public EventSubscriptionFactory(
            IWeb3 web3,
            IEventProcessingConfigurationRepository configurationRepository,
            ISubscriberQueueFactory subscriberQueueFactory = null,
            ISubscriberSearchIndexFactory subscriberSearchIndexFactory = null,
            ISubscriberStorageFactory subscriberRepositoryFactory = null):this(
                configurationRepository, 

                new EventMatcherFactory(
                    configurationRepository.ParameterConditions,
                    configurationRepository.EventSubscriptionAddresses,
                    configurationRepository.SubscriberContracts), 

                new EventHandlerFactory(
                    web3, 
                    configurationRepository, 
                    subscriberQueueFactory, 
                    subscriberSearchIndexFactory,
                    subscriberRepositoryFactory))
        {
        }

        public EventSubscriptionFactory(
            IEventProcessingConfigurationRepository configurationRepository, 
            IEventMatcherFactory eventMatcherFactory, 
            IEventHandlerFactory eventHandlerFactory)
        {
            ConfigurationRepository = configurationRepository;
            EventMatcherFactory = eventMatcherFactory;
            EventHandlerFactory = eventHandlerFactory;
        }

        public async Task<List<IEventSubscription>> LoadAsync(long partitionId)
        {
            var subscriberConfigurations = await ConfigurationRepository.Subscribers.GetManyAsync(partitionId).ConfigureAwait(false);

            var eventSubscriptions = new List<IEventSubscription>(subscriberConfigurations.Length);

            foreach (var subscriberConfiguration in subscriberConfigurations.Where(c => !c.Disabled))
            {
                var eventSubscriptionConfigurations = await ConfigurationRepository.EventSubscriptions.GetManyAsync(subscriberConfiguration.Id).ConfigureAwait(false);

                foreach (var eventSubscriptionConfig in eventSubscriptionConfigurations.Where(s => !s.Disabled))
                {
                    var eventSubscription = await LoadEventSubscriptionsAsync(eventSubscriptionConfig).ConfigureAwait(false);
                    eventSubscriptions.Add(eventSubscription);
                }
            }

            return eventSubscriptions;
        }

        private async Task<EventSubscription> LoadEventSubscriptionsAsync(IEventSubscriptionDto subscriptionConfig)
        {
            var matcher = await EventMatcherFactory.LoadAsync(subscriptionConfig).ConfigureAwait(false);
            var state = await ConfigurationRepository.EventSubscriptionStates.GetAsync(subscriptionConfig.Id).ConfigureAwait(false);
            var handlerCoOrdinator = new EventHandlerManager(ConfigurationRepository.EventHandlerHistoryRepo);

            var subscription = new EventSubscription(
                subscriptionConfig.Id, subscriptionConfig.SubscriberId, matcher, handlerCoOrdinator, state);

            await AddEventHandlers(subscription).ConfigureAwait(false);

            return subscription;
        }

        private async Task AddEventHandlers(EventSubscription eventSubscription)
        {
            var handlerConfiguration = await ConfigurationRepository.EventHandlers.GetManyAsync(eventSubscription.Id).ConfigureAwait(false);

            foreach(var configItem in handlerConfiguration.Where(c => !c.Disabled).OrderBy(h => h.Order))
            {
                eventSubscription.AddHandler(await EventHandlerFactory.LoadAsync(eventSubscription, configItem).ConfigureAwait(false));
            }

        }

    }
}
