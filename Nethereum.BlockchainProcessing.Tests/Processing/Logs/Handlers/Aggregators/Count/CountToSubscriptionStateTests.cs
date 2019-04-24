using Nethereum.BlockchainProcessing.Processing.Logs;
using Nethereum.BlockchainProcessing.Processing.Logs.Configuration;
using Nethereum.BlockchainProcessing.Processing.Logs.Handling;
using Nethereum.BlockchainProcessing.Processing.Logs.Handling.Handlers;
using System.Threading.Tasks;
using Xunit;

namespace Nethereum.BlockchainProcessing.Tests.Processing.Logs.Handlers.Aggregators.Count
{
    public class CountToSubscriptionStateTests: CountTestsBase
    {
        private const string OUTPUT_NAME = "TotalCount";

        protected override EventAggregatorConfiguration CreateConfiguration()
        {
            return new EventAggregatorConfiguration
            {
                Operation = AggregatorOperation.Count,
                Destination = AggregatorDestination.EventSubscriptionState,
                OutputName = OUTPUT_NAME
            };
        }

        [Fact]
        public override async Task CreatesAndIncrementsCounter()
        {
            for(var i = 0; i < 3; i++)
            {
                await Aggregator.HandleAsync(DecodedEvent.Empty());
            }

            Assert.Equal((uint)3, EventSubscriptionState.Values[OUTPUT_NAME]);
        }

        [Fact]
        public override async Task IncrementsExistingCounter()
        {
            EventSubscriptionState.Values[OUTPUT_NAME] = 10;

            await Aggregator.HandleAsync(DecodedEvent.Empty());

            Assert.Equal(11, EventSubscriptionState.Values[OUTPUT_NAME]);
        }
    }
}

