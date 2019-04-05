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

        private async Task<EventSubscription> LoadEventSubscriptionsAsync(EventSubscriptionDto eventSubscription)
        {
            var matcher = await EventMatcherFactory.LoadAsync(eventSubscription);
            var handler = await CreateEventHandler(eventSubscription);

            return new EventSubscription(eventSubscription.Id, eventSubscription.SubscriberId, matcher, handler);
        }

        private async Task<EventHandlerCoordinator> CreateEventHandler(EventSubscriptionDto eventSubscription)
        {
            var handlerConfiguration = await Db.GetEventHandlers(eventSubscription.Id);

            var handlers = new List<IEventHandler>(handlerConfiguration.Length);
            foreach(var configItem in handlerConfiguration.Where(c => !c.Disabled).OrderBy(h => h.Order))
            {
                handlers.Add(await DecodedEventHandlerFactory.LoadAsync(configItem));
            }

            return new EventHandlerCoordinator(
                Db,
                eventSubscription.SubscriberId, 
                eventSubscription.Id, 
                handlers);
        }

    }
}
