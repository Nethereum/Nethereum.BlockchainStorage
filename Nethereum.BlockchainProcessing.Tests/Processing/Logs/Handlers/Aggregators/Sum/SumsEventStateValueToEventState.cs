using Nethereum.BlockchainProcessing.Processing.Logs;
using Nethereum.BlockchainProcessing.Processing.Logs.Configuration;
using Nethereum.BlockchainProcessing.Processing.Logs.Handling;
using Nethereum.BlockchainProcessing.Processing.Logs.Handling.Handlers;
using System.Numerics;
using System.Threading.Tasks;
using Xunit;

namespace Nethereum.BlockchainProcessing.Tests.Processing.Logs.Handlers.Aggregators.Sum
{
    public class SumsEventStateValueToEventState: SumTestsBase
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
                Destination = AggregatorDestination.EventState,
                OutputName = OUTPUT_NAME
            };
        }

        [Fact]
        public override async Task CreatesAndIncrementsSum()
        {
            var  decodedEvent = DecodedEvent.Empty();
            decodedEvent.State[INPUT_NAME] = (BigInteger)101;

            for(var i = 0; i < 3; i++)
            {
                await Aggregator.HandleAsync(decodedEvent);
            }

            Assert.Equal((BigInteger)303, decodedEvent.State[OUTPUT_NAME]);
        }

        [Fact]
        public override async Task IncrementsExistingSum()
        {
            var  decodedEvent = DecodedEvent.Empty();
            decodedEvent.State[INPUT_NAME] = (BigInteger)101;
            decodedEvent.State[OUTPUT_NAME] = (BigInteger)202;

            await Aggregator.HandleAsync(decodedEvent);

            Assert.Equal((BigInteger)303, decodedEvent.State[OUTPUT_NAME]);
        }
    }
}

