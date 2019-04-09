using Nethereum.ABI.JsonDeserialisation;
using Nethereum.ABI.Model;
using System.Linq;
using System.Threading.Tasks;

namespace Nethereum.BlockchainProcessing.Processing.Logs.Matching
{
    public class EventMatcherFactory : IEventMatcherFactory
    {
        private readonly ABIDeserialiser _abiDeserializer = new ABIDeserialiser();

        public EventMatcherFactory(IEventProcessingConfigurationRepository configurationRepository)
        {
            _repo = configurationRepository;
        }

        private IEventProcessingConfigurationRepository _repo { get; }

        public async Task<IEventMatcher> LoadAsync(EventSubscriptionDto eventSubscription)
        {
            var eventAbis = await GetEventAbisAsync(eventSubscription);
            var addressMatcher = await CreateEventAddressMatcherAsync(eventSubscription);
            var parameterMatcher = await CreateParameterMatcherAsync(eventSubscription);
            var matcher = new EventMatcher(eventAbis, addressMatcher, parameterMatcher);
            return matcher;
        }

        private async Task<EventParameterMatcher> CreateParameterMatcherAsync(EventSubscriptionDto eventSubscription)
        {
            var parameterConditionDtos = await _repo.GetParameterConditionsAsync(eventSubscription.Id);
            var parameterConditions = parameterConditionDtos.Select(c => ParameterCondition.Create(c.ParameterOrder, c.Operator, c.Value));
            var parameterMatcher = new EventParameterMatcher(parameterConditions);
            return parameterMatcher;
        }

        private async Task<EventAddressMatcher> CreateEventAddressMatcherAsync(EventSubscriptionDto eventSubscription)
        {
            var addressDtos = await _repo.GetEventSubscriptionAddressesAsync(eventSubscription.Id);
            var addressMatcher = new EventAddressMatcher(addressDtos.Select(a => a.Address));
            return addressMatcher;
        }

        private async Task<EventABI[]> GetEventAbisAsync(EventSubscriptionDto eventSubscription)
        {
            if(eventSubscription.ContractId == null) return null;

            if (!eventSubscription.CatchAllContractEvents && eventSubscription.EventSignatures.Count == 0)
            {
                return null;
            }

            var contractDto = await _repo.GetContractAsync(eventSubscription.ContractId.Value);
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
