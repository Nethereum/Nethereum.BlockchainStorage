using Nethereum.BlockchainProcessing.Processing.Logs;
using Nethereum.BlockchainProcessing.Processing.Logs.Handling;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using System.Threading.Tasks;
using Xunit;

namespace Nethereum.BlockchainProcessing.Tests.Processing.Logs.Handlers.Aggregators.AddToList
{
    public class AddToListFromEventStateToSubscriptionState: AddToListBase
    {
        private const string OUTPUT_NAME = "CalculatedValues";
        private const string INPUT_NAME = "CalculatedValue";

        protected override EventAggregatorConfiguration CreateConfiguration()
        {
            return new EventAggregatorConfiguration
            {
                Operation = AggregatorOperation.AddToList,
                Source = AggregatorSource.EventState,
                InputName = INPUT_NAME,
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
                decodedEvent.State[INPUT_NAME] = values[i];
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
            decodedEvent.State[INPUT_NAME] = (BigInteger)101;

            await Aggregator.HandleAsync(decodedEvent);

            var list = (IList)EventSubscriptionState.Values[OUTPUT_NAME];

            Assert.Equal(2, list.Count);
            Assert.Equal((BigInteger)202, list[0]);
            Assert.Equal((BigInteger)101, list[1]);
        }
    }
}

