using Nethereum.Hex.HexTypes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;

namespace Nethereum.BlockchainProcessing.Processing.Logs.Handling
{
    public class EventAggregator : IDecodedEventHandler
    {
        public EventSubscriptionStateDto State { get; }
        public EventAggregatorConfiguration Configuration { get; }

        public EventAggregator(EventSubscriptionStateDto state, EventAggregatorConfiguration aggregatorConfiguration)
        {
            State = state;
            Configuration = aggregatorConfiguration;
        }

        public Task<bool> HandleAsync(DecodedEvent decodedEvent)
        {
            switch (Configuration.Operation)
            {
                case AggregatorOperation.Count:
                    Increment(decodedEvent);
                    return Task.FromResult(true);
                case AggregatorOperation.Sum:
                    Sum(decodedEvent);
                    return Task.FromResult(true);
                 case AggregatorOperation.AddToList:
                    AddToList(decodedEvent);
                    return Task.FromResult(true);
            }

            return Task.FromResult(true);
        }

        private void AddToList(DecodedEvent decodedEvent)
        {
            object existingValue = GetExistingValue(decodedEvent);
            object newValue = GetInputValue(decodedEvent);
            object outputValue = CreateOrAddToList(existingValue, newValue);

            StoreOutputValue(decodedEvent, outputValue);
        }

        private void Sum(DecodedEvent decodedEvent)
        {
            var inputValue = GetInputValue(decodedEvent);
            if(inputValue == null) return;

            var existingValue = GetExistingValue(decodedEvent);
            var outputValue = Sum(existingValue, inputValue);

            StoreOutputValue(decodedEvent, outputValue);
        }

        private static readonly uint Zero = (uint)0;

        private void Increment(DecodedEvent decodedEvent)
        {
            object existingValue = null;

            switch(Configuration.Destination)
            {
                case AggregatorDestination.EventSubscriptionState:
                    existingValue = GetValueFromSubscriptionState(Configuration.OutputName, Zero);
                    break;
                case AggregatorDestination.EventState:
                    existingValue = GetValueFromEventState(decodedEvent, Configuration.OutputName, Zero);
                    break;
            }   

            if(existingValue == null) return;

            var newValue = Increment(existingValue);
            StoreOutputValue(decodedEvent, newValue);
        }

        private object GetValueFromSubscriptionState(string key, object defaultValue)
        {
           if(string.IsNullOrEmpty(key)) return defaultValue;

            return State.Values.ContainsKey(key) ? 
                State.Values[key] : defaultValue;
        }

        private object GetValueFromEventState(DecodedEvent decodedEvent, string key, object defaultValue)
        {
           if(string.IsNullOrEmpty(key)) return defaultValue;

            return decodedEvent.State.ContainsKey(key) ? 
                decodedEvent.State[key] : defaultValue;
        }

        private object GetExistingValue(DecodedEvent decodedEvent)
        {
            switch (Configuration.Destination)
            {
                case AggregatorDestination.EventState:
                    return GetValueFromEventState(decodedEvent, Configuration.OutputName, null);
                case AggregatorDestination.EventSubscriptionState:
                    return GetValueFromSubscriptionState(Configuration.OutputName, null);
                default:
                    return null;
            }
        }

        private object GetInputValue(DecodedEvent decodedEvent)
        {
            switch (Configuration.Source)
            {
                case AggregatorSource.EventParameter:
                    return GetInputValueFromEventParameter(decodedEvent);
                case AggregatorSource.EventState:
                    return GetValueFromEventState(decodedEvent, Configuration.InputName, null);
                case AggregatorSource.TransactionHash:
                    return decodedEvent.EventLog.Log.TransactionHash;
                case AggregatorSource.BlockNumber:
                    return decodedEvent.EventLog.Log.BlockNumber;
                default:
                    return null;
            }
        }

        private void StoreOutputValue(DecodedEvent decodedEvent, object outputValue)
        {
            switch (Configuration.Destination)
            {
                case AggregatorDestination.EventState:
                    decodedEvent.State[Configuration.OutputName] = outputValue;
                    break;
                case AggregatorDestination.EventSubscriptionState:
                    State.Values[Configuration.OutputName] = outputValue;
                    break;
            }
        }

        private object CreateOrAddToList(object existingValue, object inputValue)
        {
            if(existingValue == null && inputValue != null)
            {
                var listType = typeof(List<>);
                var constructedListType = listType.MakeGenericType(inputValue.GetType());
                existingValue = Activator.CreateInstance(constructedListType);
            }

            if(existingValue != null && existingValue is IList li)
            {
                li.Add(inputValue);
            }

            return existingValue;
        }

        private object Increment(object existingValue)
        {
            if(existingValue == null) existingValue = (uint)0;

            if(existingValue is BigInteger bi1)
            {
                return 1 + bi1;
            }
            if(existingValue is HexBigInteger h1)
            {
                return 1 + h1.Value;
            }
            if(existingValue is uint u1)
            {
                return 1 + u1;
            }
            if(existingValue is int i1)
            {
                return 1 + i1;
            }
            if(existingValue is long l1)
            {
                return 1 + l1;
            }
            if(existingValue is ulong ul1)
            {
                return 1 + ul1;
            }
            if(existingValue is double d1)
            {
                return 1 + d1;
            }          
            return null;
        }

        private object Sum(object existingValue, object inputValue)
        {
            existingValue = existingValue ?? ((inputValue != null) ? Activator.CreateInstance(inputValue.GetType()) : null);

            if(existingValue is BigInteger bi1 && inputValue is BigInteger bi2)
            {
                return bi1 + bi2;
            }
            if(existingValue is HexBigInteger h1 && inputValue is HexBigInteger h2)
            {
                return h1.Value + h2.Value;
            }
            if(existingValue is uint u1 && inputValue is uint u2)
            {
                return u1 + u2;
            }
            if(existingValue is int i1 && inputValue is int i2)
            {
                return i1 + i2;
            }
            if(existingValue is long l1 && inputValue is long l2)
            {
                return l1 + l2;
            }
            if(existingValue is ulong ul1 && inputValue is ulong ul2)
            {
                return ul1 + ul2;
            }
            if(existingValue is double d1 && inputValue is double d2)
            {
                return d1 + d2;
            }          
            return null;
        }

        private object GetInputValueFromEventParameter(DecodedEvent decodedEvent)
        {
            if(Configuration.EventParameterNumber < 1) return null;

            var parameter = decodedEvent.EventLog.Event.FirstOrDefault(p => p.Parameter.Order == Configuration.EventParameterNumber);
            
            return parameter?.Result;
        }
    }
}
