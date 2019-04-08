using Nethereum.ABI.FunctionEncoding;
using Nethereum.ABI.Model;
using Nethereum.BlockchainProcessing.Processing.Logs;
using Nethereum.BlockchainProcessing.Processing.Logs.Handling;
using Nethereum.BlockchainProcessing.Processing.Logs.Handling.Handlers;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using System.Threading.Tasks;
using Xunit;

namespace Nethereum.BlockchainProcessing.Tests.Processing.Logs.Handlers.Aggregators.AddToList
{
    public class AddToListFromEventParameterToSubscriptionState: AddToListBase
    {
        private const string OUTPUT_NAME = "ParameterValues";
        private const int EVENT_PARAMETER_NUMBER = 3;

        protected override EventAggregatorConfiguration CreateConfiguration()
        {
             return new EventAggregatorConfiguration
            {
                Operation = AggregatorOperation.AddToList,
                Source = AggregatorSource.EventParameter,
                EventParameterNumber = EVENT_PARAMETER_NUMBER,
                Destination = AggregatorDestination.EventSubscriptionState,
                OutputName = OUTPUT_NAME
            };
        }

        [Fact]
        public override async Task CreatesAndAddsToList()
        {
            var values = new BigInteger[]{1,2,3};

            for(var i = 0; i < values.Length; i++)
            {
                var  decodedEvent = DecodedEvent.Empty();
                decodedEvent.Event.Add(
                    new ParameterOutput 
                    { 
                        Result = values[i], 
                        Parameter = new Parameter("uint256", EVENT_PARAMETER_NUMBER) 
                    });

                await Aggregator.HandleAsync(decodedEvent);
            }

            var list = (IList)EventSubscriptionState.Values[OUTPUT_NAME];

            Assert.Equal(values, list);
        }

        [Fact]
        public override async Task AddsToExistingList()
        {
            EventSubscriptionState.Values[OUTPUT_NAME] = new List<BigInteger>(new []{(BigInteger)202 });

            var  decodedEvent = DecodedEvent.Empty();
            decodedEvent.Event.Add(
                new ParameterOutput 
                { 
                    Result = (BigInteger)101, 
                    Parameter = new Parameter("uint256", EVENT_PARAMETER_NUMBER) 
                });

            await Aggregator.HandleAsync(decodedEvent);

            var list = (IList)EventSubscriptionState.Values[OUTPUT_NAME];

            Assert.Equal(2, list.Count);
            Assert.Equal((BigInteger)202, list[0]);
            Assert.Equal((BigInteger)101, list[1]);
        }
    }
}

