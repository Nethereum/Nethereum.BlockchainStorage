using Nethereum.ABI.FunctionEncoding;
using Nethereum.ABI.Model;
using Nethereum.BlockchainProcessing.Processing.Logs;
using Nethereum.BlockchainProcessing.Processing.Logs.Handling;
using Nethereum.BlockchainProcessing.Processing.Logs.Handling.Handlers;
using System.Numerics;
using System.Threading.Tasks;
using Xunit;

namespace Nethereum.BlockchainProcessing.Tests.Processing.Logs.Handlers.Aggregators.Sum
{
    public class SumsEventParameterToEventState: SumTestsBase
    {
        private const string OUTPUT_NAME = "RunningTotal";
        private const int EVENT_PARAMETER_NUMBER = 3;

        protected override EventAggregatorConfiguration CreateConfiguration()
        {
            return new EventAggregatorConfiguration
            {
                Operation = AggregatorOperation.Sum,
                Source = AggregatorSource.EventParameter,
                EventParameterNumber = EVENT_PARAMETER_NUMBER,
                Destination = AggregatorDestination.EventState,
                OutputName = OUTPUT_NAME
            };
        }

        [Fact]
        public override async Task CreatesAndIncrementsSum()
        {
            var  decodedEvent = DecodedEvent.Empty();
            decodedEvent.Event.Add(
                new ParameterOutput 
                { 
                    Result = (BigInteger)101, 
                    Parameter = new Parameter("uint256", EVENT_PARAMETER_NUMBER) 
                });

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
            decodedEvent.State[OUTPUT_NAME] = (BigInteger)202;
            decodedEvent.Event.Add(
                new ParameterOutput 
                { 
                    Result = (BigInteger)101, 
                    Parameter = new Parameter("uint256", EVENT_PARAMETER_NUMBER) 
                });

            await Aggregator.HandleAsync(decodedEvent);

            Assert.Equal((BigInteger)303, decodedEvent.State[OUTPUT_NAME]);
        }
    }
}

