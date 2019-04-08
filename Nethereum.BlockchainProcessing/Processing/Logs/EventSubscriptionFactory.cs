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

        public IEventHandlerFactory DecodedEventHandlerFactory { get; }
        public IEventProcessingConfigurationDb Db { get; }

        public EventSubscriptionFactory(
            IBlockchainProxyService blockchainProxy,
            IEventProcessingConfigurationDb db,
            ISubscriberQueueFactory subscriberQueueFactory,
            ISubscriberSearchIndexFactory subscriberSearchIndexFactory):this(
                db, 
                new EventMatcherFactory(db), 
                new EventHandlerFactory(blockchainProxy, db, subscriberQueueFactory, subscriberSearchIndexFactory))
        {
        }

        public EventSubscriptionFactory(
            IEventProcessingConfigurationDb db, 
            IEventMatcherFactory eventMatcherFactory, 
            IEventHandlerFactory decodedEventHandlerFactory)
        {
            Db = db;
            EventMatcherFactory = eventMatcherFactory;
            DecodedEventHandlerFactory = decodedEventHandlerFactory;
        }

        public async Task<List<IEventSubscription>> LoadAsync(long partitionId)
        {
            var subscriberConfigurations = await Db.GetSubscribersAsync(partitionId);

            var eventSubscriptions = new List<IEventSubscription>(subscriberConfigurations.Length);

            foreach (var subscriberConfiguration in subscriberConfigurations.Where(c => !c.Disabled))
            {
                var eventSubscriptionConfigurations = await Db.GetEventSubscriptionsAsync(subscriberConfiguration.Id);

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
            var state = await Db.GetOrCreateEventSubscriptionStateAsync(subscriptionConfig.Id);
            var handlerCoOrdinator = new EventHandlerManager(Db);

            var subscription = new EventSubscription(
                subscriptionConfig.Id, subscriptionConfig.SubscriberId, matcher, handlerCoOrdinator, state);

            await AddEventHandlers(subscription);

            return subscription;
        }

        private async Task AddEventHandlers(EventSubscription eventSubscription)
        {
            var handlerConfiguration = await Db.GetEventHandlers(eventSubscription.Id);

            foreach(var configItem in handlerConfiguration.Where(c => !c.Disabled).OrderBy(h => h.Order))
            {
                eventSubscription.AddHandler(await DecodedEventHandlerFactory.LoadAsync(eventSubscription, configItem));
            }

        }

    }
}
