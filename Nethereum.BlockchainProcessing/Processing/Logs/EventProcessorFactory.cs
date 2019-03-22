using Nethereum.ABI.JsonDeserialisation;
using Nethereum.ABI.Model;
using Nethereum.BlockchainProcessing.Processing.Logs.Handling;
using Nethereum.BlockchainProcessing.Processing.Logs.Matching;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nethereum.BlockchainProcessing.Processing.Logs
{
    public class EventProcessorFactory : ILogProcessorFactory
    {
        private readonly ABIDeserialiser _abiDeserializer = new ABIDeserialiser();
        IEventProcessingConfigurationDb _db;
        private readonly IDecodedEventHandlerFactory _decodedEventHandlerFactory;

        public EventProcessorFactory(IEventProcessingConfigurationDb db, IDecodedEventHandlerFactory decodedEventHandlerFactory)
        {
            _db = db;
            _decodedEventHandlerFactory = decodedEventHandlerFactory;
        }

        public async Task<List<ILogProcessor>> GetLogProcessorsAsync(long partitionId)
        {
            var subscribers = await _db.GetSubscribersAsync(partitionId);

            var processors = new List<ILogProcessor>(subscribers.Length);

            foreach (var subscriber in subscribers)
            {
                var eventSubscriptions = await _db.GetEventSubscriptionsAsync(subscriber.Id);

                foreach (var eventSubscription in eventSubscriptions)
                {
                    var processor = await CreateLogProcessorAsync(eventSubscription);
                    processors.Add(processor);
                }
            }

            return processors;
        }

        private async Task<LogProcessor> CreateLogProcessorAsync(EventSubscriptionDto eventSubscription)
        {
            var eventAbi = await GetEventAbiAsync(eventSubscription);
            var addressMatcher = await CreateEventAddressMatcherAsync(eventSubscription);
            var parameterMatcher = await CreateParameterMatcherAsync(eventSubscription);
            var matcher = new EventMatcher(eventAbi, addressMatcher, parameterMatcher);
            EventHandler handler = await CreateEventHandler(eventSubscription);

            var processor = new LogProcessor(matcher, handler);
            return processor;
        }

        private async Task<EventHandler> CreateEventHandler(EventSubscriptionDto eventSubscription)
        {
            var handlerConfiguration = await _db.GetDecodedEventHandlers(eventSubscription.Id);

            var handlers = new List<IDecodedEventHandler>(handlerConfiguration.Length);
            foreach(var configItem in handlerConfiguration.OrderBy(h => h.Order))
            {
                handlers.Add(await _decodedEventHandlerFactory.CreateAsync(configItem));
            }

            return new EventHandler(subscriberId: eventSubscription.SubscriberId, eventSubscriptionId: eventSubscription.Id, handlers);
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
