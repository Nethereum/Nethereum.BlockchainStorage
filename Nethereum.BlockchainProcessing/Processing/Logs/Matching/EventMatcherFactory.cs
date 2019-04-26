using Nethereum.ABI.JsonDeserialisation;
using Nethereum.ABI.Model;
using Nethereum.BlockchainProcessing.Processing.Logs.Configuration;
using System.Linq;
using System.Threading.Tasks;

namespace Nethereum.BlockchainProcessing.Processing.Logs.Matching
{
    public class EventMatcherFactory : IEventMatcherFactory
    {
        private readonly ABIDeserialiser _abiDeserializer = new ABIDeserialiser();

        public EventMatcherFactory(
            IParameterConditionRepository parameterConditionRepository,
            IEventSubscriptionAddressRepository eventSubscriptionAddressRepository,
            ISubscriberContractRepository subscriberContractRepository)
        {
            ParameterConditionRepository = parameterConditionRepository;
            EventSubscriptionAddressRepository = eventSubscriptionAddressRepository;
            SubscriberContractRepository = subscriberContractRepository;
        }

        private IEventProcessingConfigurationRepository _repo { get; }
        public IParameterConditionRepository ParameterConditionRepository { get; }
        public IEventSubscriptionAddressRepository EventSubscriptionAddressRepository { get; }
        public ISubscriberContractRepository SubscriberContractRepository { get; }

        public async Task<IEventMatcher> LoadAsync(IEventSubscriptionDto eventSubscription)
        {
            var eventAbis = await GetEventAbisAsync(eventSubscription).ConfigureAwait(false);
            var addressMatcher = await CreateEventAddressMatcherAsync(eventSubscription).ConfigureAwait(false);
            var parameterMatcher = await CreateParameterMatcherAsync(eventSubscription).ConfigureAwait(false);
            var matcher = new EventMatcher(eventAbis, addressMatcher, parameterMatcher);
            return matcher;
        }

        private async Task<EventParameterMatcher> CreateParameterMatcherAsync(IEventSubscriptionDto eventSubscription)
        {
            var parameterConditionDtos = await ParameterConditionRepository.GetManyAsync(eventSubscription.Id).ConfigureAwait(false);
            var parameterConditions = parameterConditionDtos.Select(c => ParameterCondition.Create(c.ParameterOrder, c.Operator, c.Value));
            var parameterMatcher = new EventParameterMatcher(parameterConditions);
            return parameterMatcher;
        }

        private async Task<EventAddressMatcher> CreateEventAddressMatcherAsync(IEventSubscriptionDto eventSubscription)
        {
            var addressDtos = await EventSubscriptionAddressRepository.GetManyAsync(eventSubscription.Id).ConfigureAwait(false);
            var addressMatcher = new EventAddressMatcher(addressDtos.Select(a => a.Address));
            return addressMatcher;
        }

        private async Task<EventABI[]> GetEventAbisAsync(IEventSubscriptionDto eventSubscription)
        {
            if(eventSubscription.ContractId == null) return null;

            if (!eventSubscription.CatchAllContractEvents && eventSubscription.EventSignatures.Count == 0)
            {
                return null;
            }

            var contractDto = await SubscriberContractRepository.GetAsync(eventSubscription.SubscriberId, eventSubscription.ContractId.Value).ConfigureAwait(false);
            ContractABI contractAbi = contractDto.Abi == null ? null : _abiDeserializer.DeserialiseContract(contractDto.Abi);

            if(contractAbi == null) return null;

            if (eventSubscription.CatchAllContractEvents)
            {
                return contractAbi.Events;
            }

            return contractAbi.Events.Where(e => eventSubscription.EventSignatures.Contains(e.Sha3Signature)).ToArray();
        }
    }
}
