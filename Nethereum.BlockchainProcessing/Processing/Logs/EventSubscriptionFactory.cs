using Nethereum.BlockchainProcessing.BlockchainProxy;
using Nethereum.BlockchainProcessing.Processing.Logs.Handling;
using Nethereum.BlockchainProcessing.Processing.Logs.Matching;
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
            IBlockchainProxyService blockchainProxy,
            IEventProcessingConfigurationRepository configurationRepository,
            ISubscriberQueueFactory subscriberQueueFactory = null,
            ISubscriberSearchIndexFactory subscriberSearchIndexFactory = null,
            ISubscriberRepositoryFactory subscriberRepositoryFactory = null):this(
                configurationRepository, 
                new EventMatcherFactory(configurationRepository), 
                new EventHandlerFactory(
                    blockchainProxy, 
                    configurationRepository, 
                    subscriberQueueFactory, 
                    subscriberSearchIndexFactory,
                    subscriberRepositoryFactory))
        {
        }

        public EventSubscriptionFactory(
            IEventProcessingConfigurationRepository db, 
            IEventMatcherFactory eventMatcherFactory, 
            IEventHandlerFactory decodedEventHandlerFactory)
        {
            ConfigurationRepository = db;
            EventMatcherFactory = eventMatcherFactory;
            EventHandlerFactory = decodedEventHandlerFactory;
        }

        public async Task<List<IEventSubscription>> LoadAsync(long partitionId)
        {
            var subscriberConfigurations = await ConfigurationRepository.GetSubscribersAsync(partitionId);

            var eventSubscriptions = new List<IEventSubscription>(subscriberConfigurations.Length);

            foreach (var subscriberConfiguration in subscriberConfigurations.Where(c => !c.Disabled))
            {
                var eventSubscriptionConfigurations = await ConfigurationRepository.GetEventSubscriptionsAsync(subscriberConfiguration.Id);

                foreach (var eventSubscriptionConfig in eventSubscriptionConfigurations.Where(s => !s.Disabled))
                {
                    var eventSubscription = await LoadEventSubscriptionsAsync(eventSubscriptionConfig);
                    eventSubscriptions.Add(eventSubscription);
                }
            }

            return eventSubscriptions;
        }

        private async Task<EventSubscription> LoadEventSubscriptionsAsync(EventSubscriptionDto subscriptionConfig)
        {
            var matcher = await EventMatcherFactory.LoadAsync(subscriptionConfig);
            var state = await ConfigurationRepository.GetOrCreateEventSubscriptionStateAsync(subscriptionConfig.Id);
            var handlerCoOrdinator = new EventHandlerManager(ConfigurationRepository);

            var subscription = new EventSubscription(
                subscriptionConfig.Id, subscriptionConfig.SubscriberId, matcher, handlerCoOrdinator, state);

            await AddEventHandlers(subscription);

            return subscription;
        }

        private async Task AddEventHandlers(EventSubscription eventSubscription)
        {
            var handlerConfiguration = await ConfigurationRepository.GetEventHandlers(eventSubscription.Id);

            foreach(var configItem in handlerConfiguration.Where(c => !c.Disabled).OrderBy(h => h.Order))
            {
                eventSubscription.AddHandler(await EventHandlerFactory.LoadAsync(eventSubscription, configItem));
            }

        }

    }
}
