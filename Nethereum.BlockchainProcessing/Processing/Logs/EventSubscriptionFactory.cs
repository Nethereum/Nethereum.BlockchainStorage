using Nethereum.ABI.JsonDeserialisation;
using Nethereum.ABI.Model;
using Nethereum.BlockchainProcessing.Processing.Logs.Handling;
using Nethereum.BlockchainProcessing.Processing.Logs.Matching;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nethereum.BlockchainProcessing.Processing.Logs
{
    public class EventSubscriptionFactory : IEventSubscriptionFactory
    {
        private readonly ABIDeserialiser _abiDeserializer = new ABIDeserialiser();
        IEventProcessingConfigurationDb _db;
        private readonly IEventHandlerFactory _decodedEventHandlerFactory;

        public EventSubscriptionFactory(IEventProcessingConfigurationDb db, IEventHandlerFactory decodedEventHandlerFactory)
        {
            _db = db;
            _decodedEventHandlerFactory = decodedEventHandlerFactory;
        }

        public async Task<List<IEventSubscription>> LoadAsync(long partitionId)
        {
            var subscriberConfigurations = await _db.GetSubscribersAsync(partitionId);

            var processors = new List<IEventSubscription>(subscriberConfigurations.Length);

            foreach (var subscriberConfiguration in subscriberConfigurations)
            {
                var eventSubscriptionConfigurations = await _db.GetEventSubscriptionsAsync(subscriberConfiguration.Id);

                foreach (var eventSubscriptionConfig in eventSubscriptionConfigurations)
                {
                    var eventSubscription = await LoadEventSubscriptionsAsync(eventSubscriptionConfig);
                    processors.Add(eventSubscription);
                }
            }

            return processors;
        }

        private async Task<EventSubscription> LoadEventSubscriptionsAsync(EventSubscriptionDto eventSubscription)
        {
            var eventAbi = await GetEventAbiAsync(eventSubscription);
            var addressMatcher = await CreateEventAddressMatcherAsync(eventSubscription);
            var parameterMatcher = await CreateParameterMatcherAsync(eventSubscription);
            var matcher = new EventMatcher(eventAbi, addressMatcher, parameterMatcher);

            EventHandlerCoordinator handler = await CreateEventHandler(eventSubscription);

            var processor = new EventSubscription(eventSubscription.Id, eventSubscription.SubscriberId, matcher, handler);
            return processor;
        }

        private async Task<EventHandlerCoordinator> CreateEventHandler(EventSubscriptionDto eventSubscription)
        {
            var handlerConfiguration = await _db.GetDecodedEventHandlers(eventSubscription.Id);

            var handlers = new List<IEventHandler>(handlerConfiguration.Length);
            foreach(var configItem in handlerConfiguration.OrderBy(h => h.Order))
            {
                handlers.Add(await _decodedEventHandlerFactory.CreateAsync(configItem));
            }

            return new EventHandlerCoordinator(subscriberId: eventSubscription.SubscriberId, eventSubscriptionId: eventSubscription.Id, handlers);
        }

        private async Task<EventParameterMatcher> CreateParameterMatcherAsync(EventSubscriptionDto eventSubscription)
        {
            var parameterConditionDtos = await _db.GetParameterConditionsAsync(eventSubscription.Id);
            var parameterConditions = parameterConditionDtos.Select(c => ParameterCondition.Create(c.ParameterOrder, c.Operator, c.Value));
            var parameterMatcher = new EventParameterMatcher(parameterConditions);
            return parameterMatcher;
        }

        private async Task<EventAddressMatcher> CreateEventAddressMatcherAsync(EventSubscriptionDto eventSubscription)
        {
            var addressDtos = await _db.GetEventAddressesAsync(eventSubscription.Id);
            var addressMatcher = new EventAddressMatcher(addressDtos.Select(a => a.Address));
            return addressMatcher;
        }

        private async Task<EventABI> GetEventAbiAsync(EventSubscriptionDto eventSubscription)
        {
            if (eventSubscription.ContractId == null || eventSubscription.EventSignature == null)
            {
                return null;
            }

            var contractDto = await _db.GetContractAsync(eventSubscription.ContractId.Value);

            ContractABI contractAbi = contractDto.Abi == null ? null : _abiDeserializer.DeserialiseContract(contractDto.Abi);
            EventABI eventAbi = contractAbi == null ? null : contractAbi.Events.FirstOrDefault(e => e.Sha3Signature == eventSubscription.EventSignature);
            return eventAbi;
        }


    }
}
