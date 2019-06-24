using Nethereum.BlockchainProcessing.Processing.Logs;
using Nethereum.BlockchainProcessing.Processing.Logs.Configuration;
using Nethereum.BlockchainProcessing.Processing.Logs.Handling;
using Nethereum.BlockchainProcessing.Processing.Logs.Handling.Handlers;
using Nethereum.BlockchainProcessing.Tests.Processing.Logs.Handlers.Aggregators.Count;
using System.Threading.Tasks;
using Xunit;

namespace Nethereum.LogProcessing.Tests.Handlers.Aggregators.Count
{
    public class CountToEventStateTests : CountTestsBase
    {
        private const string OUTPUT_NAME = "TotalCount";

        protected override IEventAggregatorDto CreateConfiguration()
        {
            return new EventAggregatorDto
            {
                Operation = AggregatorOperation.Count,
                Destination = AggregatorDestination.EventState,
                OutputKey = OUTPUT_NAME
            };
        }

        [Fact]
        public override async Task CreatesAndIncrementsCounter()
        {
            var decodedEvent = DecodedEvent.Empty();

            for (var i = 0; i < 3; i++)
            {
                await Aggregator.HandleAsync(decodedEvent);
            }

            Assert.Equal((uint)3, decodedEvent.State[OUTPUT_NAME]);
        }

        [Fact]
        public override async Task IncrementsExistingCounter()
        {
            var decodedEvent = DecodedEvent.Empty();
            decodedEvent.State[OUTPUT_NAME] = 10;

            for (var i = 0; i < 3; i++)
            {
                await Aggregator.HandleAsync(decodedEvent);
            }

            Assert.Equal(13, decodedEvent.State[OUTPUT_NAME]);
        }
    }
}

