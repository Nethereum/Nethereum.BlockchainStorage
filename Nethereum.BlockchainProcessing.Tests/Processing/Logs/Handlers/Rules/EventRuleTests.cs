using Moq;
using Nethereum.ABI.Model;
using Nethereum.BlockchainProcessing.Processing.Logs;
using Nethereum.BlockchainProcessing.Processing.Logs.Configuration;
using Nethereum.BlockchainProcessing.Processing.Logs.Handling;
using Nethereum.BlockchainProcessing.Processing.Logs.Handling.Handlers;
using Nethereum.Hex.HexTypes;
using Nethereum.RPC.Eth.DTOs;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using Xunit;

namespace Nethereum.BlockchainProcessing.Tests.Processing.Logs.Handlers.Rules
{
    public class EventRuleFromParameterTests
    {
        EventSubscriptionStateDto _eventSubscriptionState = new EventSubscriptionStateDto();
        FilterLog _sampleTransferLog = TestData.Contracts.StandardContract.SampleTransferLog();
        EventABI _transferEventAbi = TestData.Contracts.StandardContract.TransferEventAbi;
        Mock<IEventSubscription> _mockEventSubscription = new Mock<IEventSubscription>();

        [Fact]
        public async Task IsEmpty()
        {
            var config = new EventRuleConfiguration
            {
                Type = EventRuleType.Empty,
                Source = EventRuleSource.EventParameter,
                EventParameterNumber = 1
            };

            var rule = new EventRule(_mockEventSubscription.Object, 1, config);

            var decodedEvent = _sampleTransferLog.ToDecodedEvent(_transferEventAbi);
            Assert.False(await rule.HandleAsync(decodedEvent));

            decodedEvent.Event.First(p => p.Parameter.Order == config.EventParameterNumber).Result = null;
            Assert.True(await rule.HandleAsync(decodedEvent));

            decodedEvent.Event.First(p => p.Parameter.Order == config.EventParameterNumber).Result = string.Empty;
            Assert.True(await rule.HandleAsync(decodedEvent));
        }

        [Fact]
        public async Task IsEqual_ForStrings()
        {
            var valueToFind = "xyz";

            await IsEqual(val: valueToFind, valAsString: valueToFind, differentValue: "abc");
        }

        [Fact]
        public async Task IsEqual_ForBigIntegers()
        {
            var valueToFind = BigInteger.Parse("123456789123456789123456");

            await IsEqual(val: valueToFind, valAsString: valueToFind.ToString(), differentValue: valueToFind + 1);
        }

        [Fact]
        public async Task IsEqual_ForHexBigIntegers()
        {
            var valueToFind = new HexBigInteger(BigInteger.Parse("123456789123456789123456"));

            await IsEqual(val: valueToFind, valAsString: valueToFind.Value.ToString(), differentValue: new HexBigInteger(valueToFind.Value + 1));
        }

        [Fact]
        public async Task IsEqual_ForLong()
        {
            long valueToFind = 123456789;

            await IsEqual(val: valueToFind, valAsString: valueToFind.ToString(), differentValue: valueToFind + 1);
        }

        [Fact]
        public async Task IsEqual_ForInt()
        {
            int valueToFind = 123456789;
            await IsEqual(val: valueToFind, valAsString: valueToFind.ToString(), differentValue: valueToFind + 1);
        }

        private async Task IsEqual(object val, string valAsString, object differentValue)
        {
            var config = new EventRuleConfiguration
            {
                Type = EventRuleType.Equals,
                Source = EventRuleSource.EventParameter,
                EventParameterNumber = 1,
                Value = valAsString
            };

            var rule = new EventRule(_mockEventSubscription.Object, 1, config);

            var decodedEvent = _sampleTransferLog.ToDecodedEvent(_transferEventAbi);
            decodedEvent.Event.First(p => p.Parameter.Order == config.EventParameterNumber).Result = val;
            Assert.True(await rule.HandleAsync(decodedEvent));

            decodedEvent.Event.First(p => p.Parameter.Order == config.EventParameterNumber).Result = null;
            Assert.False(await rule.HandleAsync(decodedEvent));

            decodedEvent.Event.First(p => p.Parameter.Order == config.EventParameterNumber).Result = differentValue;
            Assert.False(await rule.HandleAsync(decodedEvent));
        }

        [Fact]
        public async Task IsGreaterOrEqual_BigInteger()
        {
            var minValue = BigInteger.Parse("123456789123456789123456");

            await IsGreaterOrEqual(
                minValue: minValue,
                minValueAsString: minValue.ToString(),
                higherVal: minValue + 1,
                lowerVal: minValue - 1);
        }

        [Fact]
        public async Task IsGreaterOrEqual_Int()
        {
            int minValue = 123456789;

            await IsGreaterOrEqual(
                minValue: minValue,
                minValueAsString: minValue.ToString(),
                higherVal: minValue + 1,
                lowerVal: minValue - 1);
        }

        [Fact]
        public async Task IsGreaterOrEqual_Long()
        {
            long minValue = 123456789123;

            await IsGreaterOrEqual(
                minValue: minValue,
                minValueAsString: minValue.ToString(),
                higherVal: minValue + 1,
                lowerVal: minValue - 1);
        }

        [Fact]
        public async Task IsGreaterOrEqual_HexBigInteger()
        {
            var minValue = new HexBigInteger(BigInteger.Parse("123456789123456789123456"));

            await IsGreaterOrEqual(
                minValue: minValue, 
                minValueAsString: minValue.Value.ToString(), 
                higherVal: new HexBigInteger(minValue.Value + 1), 
                lowerVal: new HexBigInteger(minValue.Value - 1));
        }

        private async Task IsGreaterOrEqual(object minValue, string minValueAsString, object higherVal, object lowerVal)
        {

            var config = new EventRuleConfiguration
            {
                Type = EventRuleType.GreaterOrEqualTo,
                Source = EventRuleSource.EventParameter,
                EventParameterNumber = 1,
                Value = minValueAsString
            };

            var rule = new EventRule(_mockEventSubscription.Object, 1, config);

            var decodedEvent = _sampleTransferLog.ToDecodedEvent(_transferEventAbi);
            decodedEvent.Event.First(p => p.Parameter.Order == config.EventParameterNumber).Result = minValue;
            Assert.True(await rule.HandleAsync(decodedEvent));

            decodedEvent.Event.First(p => p.Parameter.Order == config.EventParameterNumber).Result = higherVal;
            Assert.True(await rule.HandleAsync(decodedEvent));

            decodedEvent.Event.First(p => p.Parameter.Order == config.EventParameterNumber).Result = null;
            Assert.False(await rule.HandleAsync(decodedEvent));

            decodedEvent.Event.First(p => p.Parameter.Order == config.EventParameterNumber).Result = lowerVal;
            Assert.False(await rule.HandleAsync(decodedEvent));
        }

        [Fact]
        public async Task IsLessOrEqual_BigInteger()
        {
            var maxValue = BigInteger.Parse("123456789123456789123456");

            await IsLessOrEqual(
                maxValue: maxValue,
                maxValueAsString: maxValue.ToString(),
                lesserValue: maxValue - 1,
                higherValue: maxValue + 1);
        }

        [Fact]
        public async Task IsLessOrEqual_Int()
        {
            int maxValue = 12345678;

            await IsLessOrEqual(
                maxValue: maxValue,
                maxValueAsString: maxValue.ToString(),
                lesserValue: maxValue - 1,
                higherValue: maxValue + 1);
        }

        [Fact]
        public async Task IsLessOrEqual_Long()
        {
            long maxValue = 1234567812345123;

            await IsLessOrEqual(
                maxValue: maxValue,
                maxValueAsString: maxValue.ToString(),
                lesserValue: maxValue - 1,
                higherValue: maxValue + 1);
        }

        [Fact]
        public async Task IsLessOrEqual_HexBigInteger()
        {
            var maxValue = new HexBigInteger(BigInteger.Parse("1234567812345"));

            await IsLessOrEqual(
                maxValue: maxValue, 
                maxValueAsString: maxValue.Value.ToString(), 
                lesserValue: new HexBigInteger(maxValue.Value - 1), 
                higherValue: new HexBigInteger(maxValue.Value + 1));
        }

        private async Task IsLessOrEqual(object maxValue, string maxValueAsString, object higherValue, object lesserValue)
        {
            var config = new EventRuleConfiguration
            {
                Type = EventRuleType.LessThanOrEqualTo,
                Source = EventRuleSource.EventParameter,
                EventParameterNumber = 1,
                Value = maxValueAsString
            };

            var rule = new EventRule(_mockEventSubscription.Object, 1, config);

            var decodedEvent = _sampleTransferLog.ToDecodedEvent(_transferEventAbi);
            decodedEvent.Event.First(p => p.Parameter.Order == config.EventParameterNumber).Result = maxValue;
            Assert.True(await rule.HandleAsync(decodedEvent));

            decodedEvent.Event.First(p => p.Parameter.Order == config.EventParameterNumber).Result = lesserValue;
            Assert.True(await rule.HandleAsync(decodedEvent));

            decodedEvent.Event.First(p => p.Parameter.Order == config.EventParameterNumber).Result = null;
            Assert.False(await rule.HandleAsync(decodedEvent));

            decodedEvent.Event.First(p => p.Parameter.Order == config.EventParameterNumber).Result = higherValue;
            Assert.False(await rule.HandleAsync(decodedEvent));
        }

        [Fact]
        public async Task Modulus_Int()
        {
            var modulus = 10;
            await IsModulus(modulus, modulus.ToString(), 100, 101);
        }

        [Fact]
        public async Task Modulus_Long()
        {
            long modulus = 10;
            await IsModulus(modulus, modulus.ToString(), (long)100, (long)101);
        }

        [Fact]
        public async Task Modulus_BigInteger()
        {
            var modulus = BigInteger.Parse("10");
            await IsModulus(modulus, modulus.ToString(), BigInteger.Parse("100"), BigInteger.Parse("101"));
        }

        [Fact]
        public async Task Modulus_HexBigInteger()
        {
            var modulus = new HexBigInteger(BigInteger.Parse("10"));
            await IsModulus(modulus, modulus.Value.ToString(), new HexBigInteger(BigInteger.Parse("100")), new HexBigInteger(BigInteger.Parse("101")));
        }

        private async Task IsModulus(object modulusOf, string modulusAsString, object isAModulus, object notAModulus)
        {

            var config = new EventRuleConfiguration
            {
                Type = EventRuleType.Modulus,
                Source = EventRuleSource.EventParameter,
                EventParameterNumber = 1,
                Value = modulusAsString
            };

            var rule = new EventRule(_mockEventSubscription.Object, 1, config);

            var decodedEvent = _sampleTransferLog.ToDecodedEvent(_transferEventAbi);
            decodedEvent.Event.First(p => p.Parameter.Order == config.EventParameterNumber).Result = modulusOf;
            Assert.True(await rule.HandleAsync(decodedEvent));

            decodedEvent.Event.First(p => p.Parameter.Order == config.EventParameterNumber).Result = isAModulus;
            Assert.True(await rule.HandleAsync(decodedEvent));

            decodedEvent.Event.First(p => p.Parameter.Order == config.EventParameterNumber).Result = null;
            Assert.False(await rule.HandleAsync(decodedEvent));

            decodedEvent.Event.First(p => p.Parameter.Order == config.EventParameterNumber).Result = notAModulus;
            Assert.False(await rule.HandleAsync(decodedEvent));
        }
    }
}
