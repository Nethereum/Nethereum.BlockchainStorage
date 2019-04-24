using Nethereum.BlockchainProcessing.Processing.Logs;
using Nethereum.BlockchainProcessing.Processing.Logs.Configuration;
using Nethereum.BlockchainProcessing.Processing.Logs.Handling;
using Nethereum.BlockchainProcessing.Processing.Logs.Handling.Handlers;
using System.Numerics;
using System.Threading.Tasks;
using Xunit;

namespace Nethereum.BlockchainProcessing.Tests.Processing.Logs.Handlers.Aggregators.Sum
{
    public class SumsEventStateValueToSuscriptionState: SumTestsBase
    {
        private const string OUTPUT_NAME = "RunningTotal";
        private const string INPUT_NAME = "Val";

        protected override EventAggregatorConfiguration CreateConfiguration()
        {
            return new EventAggregatorConfiguration
            {
                Operation = AggregatorOperation.Sum,
                Source = AggregatorSource.EventState,
                InputName = INPUT_NAME,
                Destination = AggregatorDestination.EventSubscriptionState,
                OutputName = OUTPUT_NAME
            };
        }

        [Fact]
        public override async Task CreatesAndIncrementsSum()
        {
            for(var i = 0; i < 3; i++)
            {
                var  decodedEvent = DecodedEvent.Empty();
                decodedEvent.State[INPUT_NAME] = (BigInteger)101;
                await Aggregator.HandleAsync(decodedEvent);
            }

            Assert.Equal((BigInteger)303, EventSubscriptionState.Values[OUTPUT_NAME]);
        }

        [Fact]
        public override async Task IncrementsExistingSum()
        {
            EventSubscriptionState.Values[OUTPUT_NAME] = (BigInteger)202;

            var  decodedEvent = DecodedEvent.Empty();
            decodedEvent.State[INPUT_NAME] = (BigInteger)101;

            await Aggregator.HandleAsync(decodedEvent);

            Assert.Equal((BigInteger)303, EventSubscriptionState.Values[OUTPUT_NAME]);
        }
    }
}

