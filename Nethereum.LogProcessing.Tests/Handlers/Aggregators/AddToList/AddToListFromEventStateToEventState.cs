using Nethereum.BlockchainProcessing.Processing.Logs;
using Nethereum.BlockchainProcessing.Processing.Logs.Configuration;
using Nethereum.BlockchainProcessing.Processing.Logs.Handling;
using Nethereum.BlockchainProcessing.Processing.Logs.Handling.Handlers;
using Nethereum.BlockchainProcessing.Tests.Processing.Logs.Handlers.Aggregators.AddToList;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using System.Threading.Tasks;
using Xunit;

namespace Nethereum.LogProcessing.Tests.Handlers.Aggregators.AddToList
{
    public class AddToListFromEventStateToEventState : AddToListBase
    {
        private const string OUTPUT_NAME = "CalculatedValues";
        private const string INPUT_NAME = "CalculatedValue";

        protected override IEventAggregatorDto CreateConfiguration()
        {
            return new EventAggregatorDto
            {
                Operation = AggregatorOperation.AddToList,
                Source = AggregatorSource.EventState,
                SourceKey = INPUT_NAME,
                Destination = AggregatorDestination.EventState,
                OutputKey = OUTPUT_NAME
            };
        }

        [Fact]
        public override async Task CreatesAndAddsToList()
        {
            var values = new BigInteger[] { 1, 2, 3 };

            var decodedEvent = DecodedEvent.Empty();

            for (var i = 0; i < values.Length; i++)
            {
                decodedEvent.State[INPUT_NAME] = values[i];
                await Aggregator.HandleAsync(decodedEvent);
            }

            var list = (IList)decodedEvent.State[OUTPUT_NAME];

            Assert.Equal(values, list);
        }

        [Fact]
        public override async Task AddsToExistingList()
        {
            var decodedEvent = DecodedEvent.Empty();
            decodedEvent.State[OUTPUT_NAME] = new List<BigInteger>(new[] { (BigInteger)202 });
            decodedEvent.State[INPUT_NAME] = (BigInteger)101;

            await Aggregator.HandleAsync(decodedEvent);

            var list = (IList)decodedEvent.State[OUTPUT_NAME];

            Assert.Equal(2, list.Count);
            Assert.Equal((BigInteger)202, list[0]);
            Assert.Equal((BigInteger)101, list[1]);
        }
    }
}

