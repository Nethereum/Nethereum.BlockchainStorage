using Nethereum.ABI.FunctionEncoding;
using Nethereum.ABI.Model;
using Nethereum.BlockchainProcessing.Processing.Logs;
using Nethereum.BlockchainProcessing.Processing.Logs.Handling;
using System.Numerics;
using System.Threading.Tasks;
using Xunit;

namespace Nethereum.BlockchainProcessing.Tests.Processing.Logs.HandlerTests.ContractQueryTests
{
    public class ContractQueryMultiFunctionInputTests : ContractQueryBaseTest
    {
        private object[] ActualFunctionInputArgs => actualQueryArgs.FunctionInputValues;
        protected override object FAKE_QUERY_RESULT => true;
        private const string EVENT_STATE_QUERY_OUTPUT_NAME = "EIRC20_Approve_Result";
        private const string APPROVE_ADDRESS = "0x98c1301520edff0bb14314c64987a71fa5efa407";
        private static BigInteger APPROVE_AMOUNT = new BigInteger(100);

        public ContractQueryMultiFunctionInputTests():base(new ContractQueryConfiguration
            {
                ContractABI = TestData.Contracts.StandardContract.Abi,
                ContractAddressSource = ContractAddressSource.Static,
                ContractAddress = CONTRACT_ADDRESS,
                FunctionSignature = SHA3_FUNCTION_SIGNATURES.APPROVE,
                EventStateOutputName = EVENT_STATE_QUERY_OUTPUT_NAME
        })
        {
        }

        [Fact]
        public async Task CanBeStatic()
        {
            queryConfig.Parameters = new[]
            {
                new ContractQueryParameter{
                    Order = 1,
                    Source = EventValueSource.Static,
                    Value = APPROVE_ADDRESS},
                new ContractQueryParameter{
                    Order = 2,
                    Source = EventValueSource.Static,
                    Value = APPROVE_AMOUNT}
            };

            Assert.True(await contractQueryEventHandler.HandleAsync(decodedEvent));

            Verify();

        }

        [Fact]
        public async Task CanBeFromEventState()
        {
            queryConfig.Parameters = new[]
            {
                new ContractQueryParameter{
                    Order = 1, 
                    Source = EventValueSource.EventState, 
                    EventStateName = "APPROVE_ADDRESS"},
                new ContractQueryParameter{
                    Order = 2, 
                    Source = EventValueSource.EventState, 
                    EventStateName = "APPROVE_AMOUNT"}
            };

            decodedEvent.State["APPROVE_ADDRESS"] = APPROVE_ADDRESS;
            decodedEvent.State["APPROVE_AMOUNT"] = APPROVE_AMOUNT;

            Assert.True(await contractQueryEventHandler.HandleAsync(decodedEvent));

            Verify();
            
        }

        [Fact]
        public async Task CanBeFromEventParameters()
        {
            queryConfig.Parameters = new[]
            {
                new ContractQueryParameter{
                    Order = 1, 
                    Source = EventValueSource.EventParameters, 
                    EventParameterNumber = 1},
                new ContractQueryParameter{
                    Order = 2, 
                    Source = EventValueSource.EventParameters, 
                    EventParameterNumber = 2}
            };

            decodedEvent.EventLog.Event.Add(
                new ParameterOutput{
                    Result = APPROVE_ADDRESS, Parameter = new Parameter("address", 1) });
            decodedEvent.EventLog.Event.Add(
                new ParameterOutput{
                    Result = APPROVE_AMOUNT, Parameter = new Parameter("uint256", 2) });

            Assert.True(await contractQueryEventHandler.HandleAsync(decodedEvent));

            Verify();
            
        }

        [Fact]
        public async Task CanHaveMixedSources()
        {
            queryConfig.Parameters = new[]
            {
                new ContractQueryParameter{
                    Order = 1, 
                    Source = EventValueSource.Static,
                    Value = APPROVE_ADDRESS},
                new ContractQueryParameter{
                    Order = 2, 
                    Source = EventValueSource.EventParameters, 
                    EventParameterNumber = 2}
            };

            decodedEvent.EventLog.Event.Add(
                new ParameterOutput{
                    Result = APPROVE_AMOUNT, Parameter = new Parameter("uint256", 2) });

            Assert.True(await contractQueryEventHandler.HandleAsync(decodedEvent));

            Verify();
            
        }

        private void Verify()
        {
            Assert.Equal(APPROVE_ADDRESS, ActualFunctionInputArgs[0]);
            Assert.Equal(APPROVE_AMOUNT, ActualFunctionInputArgs[1]);
            Assert.Equal(FAKE_QUERY_RESULT, decodedEvent.State[EVENT_STATE_QUERY_OUTPUT_NAME]);
        }

    }
}
