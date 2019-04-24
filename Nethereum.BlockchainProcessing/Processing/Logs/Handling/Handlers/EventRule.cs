using Nethereum.BlockchainProcessing.Processing.Logs.Configuration;
using Nethereum.Hex.HexTypes;
using System;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;

namespace Nethereum.BlockchainProcessing.Processing.Logs.Handling.Handlers
{

    public class EventRule : EventHandlerBase, IEventHandler
    {
        public EventRule(IEventSubscription subscription, long id, IEventRuleDto configuration) :base(subscription, id)
        {
            Configuration = configuration;
        }

        public IEventRuleDto Configuration { get; }

        public Task<bool> HandleAsync(DecodedEvent decodedEvent)
        {
            var inputValue = GetInputValue(decodedEvent);

            var isTrue = IsTrue(inputValue);

            return Task.FromResult(isTrue);
        }

        private bool IsTrue(object inputValue)
        {
            switch (Configuration.Type)
            {
                case EventRuleType.Empty:
                    return IsEmpty(inputValue);
                case EventRuleType.Equals:
                    return IsEqual(inputValue);
                case EventRuleType.GreaterOrEqualTo:
                    return IsGreatorOrEqualTo(inputValue);
                case EventRuleType.LessThanOrEqualTo:
                    return IsLessThanOrEqualTo(inputValue);
                case EventRuleType.Modulus:
                    return IsModulus(inputValue);
                default:
                    return false;
            }
        }

        private bool IsModulus(object inputValue)
        {
            if (inputValue == null) return false;

            if(!long.TryParse(Configuration.Value, out long modulus))
            {
                return false;
            }

            if(modulus == 0) return false;

            if (inputValue is int inputValAsInt)
            {
                return (inputValAsInt % modulus) == 0;
            }

            if (inputValue is long inputValAsLong)
            {
                return (inputValAsLong % modulus) == 0;
            }

            if (inputValue is BigInteger inputValAsBigInteger)
            {
                return (inputValAsBigInteger % modulus) == 0;
            }

            if (inputValue is HexBigInteger inputValAsHexBigInt)
            {
                return (inputValAsHexBigInt.Value % modulus) == 0;
            }

            return false;
        }

        private bool IsLessThanOrEqualTo(object inputValue)
        {
            if (inputValue == null) return false;

            if (!BigInteger.TryParse(Configuration.Value, out BigInteger valAsBi))
            {
                return false;
            }

            if (inputValue is int inputValAsInt)
            {
                return inputValAsInt <= valAsBi;
            }

            if (inputValue is long inputValAsLong)
            {
                return inputValAsLong <= valAsBi;
            }

            if (inputValue is BigInteger inputValAsBigInteger)
            {
                return inputValAsBigInteger <= valAsBi;
            }

            if (inputValue is HexBigInteger inputValAsHexBigInt)
            {
                return inputValAsHexBigInt.Value <= valAsBi;
            }

            return false;
        }

        private bool IsGreatorOrEqualTo(object inputValue)
        {
            if (inputValue == null) return false;

            if (!BigInteger.TryParse(Configuration.Value, out BigInteger valAsBi))
            {
                return false;
            }

            if (inputValue is int inputValAsInt)
            {
                return inputValAsInt >= valAsBi;
            }

            if (inputValue is long inputValAsLong)
            {
                return inputValAsLong >= valAsBi;
            }

            if (inputValue is BigInteger inputValAsBigInteger)
            {
                return inputValAsBigInteger >= valAsBi;
            }

            if (inputValue is HexBigInteger inputValAsHexBigInt)
            {
                return inputValAsHexBigInt.Value >= valAsBi;
            }

            return false;
        }

        private bool IsEqual(object inputValue)
        {
            if(inputValue == null) return false;

            if(inputValue is String s)
            {
                return s == Configuration.Value;
            }

            if (!BigInteger.TryParse(Configuration.Value, out BigInteger valAsBi))
            {
                return false;
            }

            if (inputValue is int inputValAsInt)
            {
                return valAsBi == inputValAsInt;
            }

            if (inputValue is long inputValAsLong)
            {
                return valAsBi == inputValAsLong;
            }

            if (inputValue is BigInteger inputValAsBigInteger)
            {
                return valAsBi == inputValAsBigInteger;
            }

            if (inputValue is HexBigInteger inputValAsHexBigInt)
            {
                return valAsBi == inputValAsHexBigInt.Value;
            }

            return false;
        }

        private bool IsEmpty(object inputValue)
        {
            if(inputValue == null) return true;
            if(inputValue is string str) return string.IsNullOrEmpty(str);

            return false;
        }

        private object GetInputValue(DecodedEvent decodedEvent)
        {
            switch (Configuration.Source)
            {
                case EventRuleSource.Static:
                    return Configuration.Value;
                case EventRuleSource.EventState:
                    return GetValueFromEventState(decodedEvent);
                case EventRuleSource.EventSubscriptionState:
                    return GetValueFromEventSubscriptionState();
                case EventRuleSource.EventParameter:
                    return GetEventParameterValue(decodedEvent);
                default:
                    return null;
            }
        }

        private object GetValueFromEventSubscriptionState()
        {
            Subscription.State.Values.TryGetValue(Configuration.InputName, out object val);
            return val;
        }

        private object GetValueFromEventState(DecodedEvent decodedEvent)
        {
            decodedEvent.State.TryGetValue(Configuration.InputName, out object val);
            return val;
        }

        private object GetEventParameterValue(DecodedEvent decodedEvent)
        {
            var parameter = decodedEvent.Event?.FirstOrDefault(p => p.Parameter.Order == Configuration.EventParameterNumber);
            return parameter?.Result;
        }
    }
}
