using Moq;
using Nethereum.BlockchainProcessing.Processing.Logs;
using Nethereum.BlockchainProcessing.Processing.Logs.Matching;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Nethereum.BlockchainProcessing.Tests.Processing.Logs
{
    public class EventMatcherFactoryTest
    {
        EventMatcherFactory _factory;
        Mock<IEventProcessingConfigurationRepository> _mockDb;

        SubscriberDto _subscriberOneConfig;
        SubscriberContractDto _contractDto;
        EventSubscriptionDto _eventSubscriptionConfig;
        ParameterConditionDto _parameterConditionConfig;
        EventSubscriptionAddressDto _addressesConfig;

        public EventMatcherFactoryTest()
        {
            _mockDb = new Mock<IEventProcessingConfigurationRepository>();
            _factory = new EventMatcherFactory(_mockDb.Object);

            _subscriberOneConfig = new SubscriberDto{Id = 1};
            _contractDto = new SubscriberContractDto
            {
                Id = 1,
                SubscriberId = _subscriberOneConfig.Id,
                Abi = TestData.Contracts.StandardContract.Abi,
                Name = "Transfer"
            };
            _eventSubscriptionConfig = new EventSubscriptionDto
            {
                Id = 1, 
                SubscriberId = _subscriberOneConfig.Id, 
                ContractId = _contractDto.Id, 
                EventSignature = TestData.Contracts.StandardContract.TransferEventSignature
            };
            _addressesConfig = new EventSubscriptionAddressDto
            {
                Id = 1,
                Address = "",
                EventSubscriptionId = _eventSubscriptionConfig.Id
            };
            _parameterConditionConfig = new ParameterConditionDto
            {
                Id = 1,
                EventSubscriptionId = _eventSubscriptionConfig.Id,
                ParameterOrder = 1,
                Operator = ParameterConditionOperator.Equals,
                Value = "xyz"
            };

            _mockDb.Setup(d => d.GetContractAsync(_contractDto.Id)).ReturnsAsync(_contractDto);
            _mockDb.Setup(d => d.GetEventSubscriptionsAsync(_subscriberOneConfig.Id)).ReturnsAsync(new []{_eventSubscriptionConfig});
            _mockDb.Setup(d => d.GetEventSubscriptionAddressesAsync(_eventSubscriptionConfig.Id)).ReturnsAsync(new []{_addressesConfig});
            _mockDb.Setup(d => d.GetParameterConditionsAsync(_eventSubscriptionConfig.Id)).ReturnsAsync(new []{_parameterConditionConfig});
        }

        [Fact]
        public async Task LoadsMatcherFromConfig()
        {
            var eventMatcher = await _factory.LoadAsync(_eventSubscriptionConfig) as EventMatcher;

            Assert.NotNull(eventMatcher);
            Assert.Equal(TestData.Contracts.StandardContract.TransferEventSignature, eventMatcher.Abi.Sha3Signature);

            var eventAddressMatcher = eventMatcher.AddressMatcher as EventAddressMatcher;
            Assert.Single(eventAddressMatcher.AddressesToMatch);
            Assert.Contains(_addressesConfig.Address, eventAddressMatcher.AddressesToMatch);

            var parameterMatcher = eventMatcher.ParameterMatcher as EventParameterMatcher;
            Assert.Single(parameterMatcher.ParameterConditions);
            var parameterEquals = parameterMatcher.ParameterConditions.First() as ParameterEquals;
            Assert.Equal(_parameterConditionConfig.ParameterOrder, parameterEquals.ParameterOrder);
            Assert.Equal(_parameterConditionConfig.Value, parameterEquals.ExpectedValue);
        }

    }
}
