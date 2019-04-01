using Nethereum.ABI.JsonDeserialisation;
using Nethereum.ABI.Model;
using System.Linq;
using System.Threading.Tasks;

namespace Nethereum.BlockchainProcessing.Processing.Logs.Matching
{
    public class EventMatcherFactory : IEventMatcherFactory
    {
        private readonly ABIDeserialiser _abiDeserializer = new ABIDeserialiser();

        public EventMatcherFactory(IEventProcessingConfigurationDb db)
        {
            _db = db;
        }

        private IEventProcessingConfigurationDb _db { get; }

        public async Task<IEventMatcher> LoadAsync(EventSubscriptionDto eventSubscription)
        {
            var eventAbi = await GetEventAbiAsync(eventSubscription);
            var addressMatcher = await CreateEventAddressMatcherAsync(eventSubscription);
            var parameterMatcher = await CreateParameterMatcherAsync(eventSubscription);
            var matcher = new EventMatcher(eventAbi, addressMatcher, parameterMatcher);
            return matcher;
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
            var addressDtos = await _db.GetEventSubscriptionAddressesAsync(eventSubscription.Id);
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
