using Nethereum.ABI.FunctionEncoding;
using Nethereum.ABI.Model;
using Nethereum.BlockchainProcessing.Processing.Logs;
using Nethereum.BlockchainProcessing.Processing.Logs.Handling;
using System.Numerics;
using System.Threading.Tasks;
using Xunit;

namespace Nethereum.BlockchainProcessing.Tests.Processing.Logs.HandlerTests.ContractQueryTests
{
    public class ContractQueryFunctionInputTests : ContractQueryBaseTest
    {
        private object[] ActualFunctionInputArgs => actualQueryArgs.FunctionInputValues;
        protected override object FAKE_QUERY_RESULT => BigInteger.One;

        private const string EVENT_STATE_QUERY_OUTPUT_NAME = "EIRC20_Balance";

        public ContractQueryFunctionInputTests():base(new ContractQueryConfiguration
            {
                ContractABI = TestData.Contracts.StandardContract.Abi,
                ContractAddressSource = ContractAddressSource.Static,
                ContractAddress = CONTRACT_ADDRESS,
                FunctionSignature = SHA3_FUNCTION_SIGNATURES.BALANCE_OF,
                EventStateOutputName = EVENT_STATE_QUERY_OUTPUT_NAME
        })
        {
        }

        [Fact]
        public async Task CanBeFromEventParameter()
        {
            queryConfig.Parameters = new[]
            {
                new ContractQueryParameter{
                    Order = 1, 
                    Source = EventValueSource.EventParameters, 
                    EventParameterNumber = 1}
            };

            decodedEvent.Event.Add(
                new ParameterOutput 
                { 
                    Result = OWNER_ADDRESS, 
                    Parameter = new Parameter("address", 1) 
                });

            var result = await contractQueryEventHandler.HandleAsync(decodedEvent);

            Verify(result);
        }

        [Fact]
        public async Task CanBeAStaticValue()
        {
            queryConfig.Parameters = new[]
            {
                new ContractQueryParameter{
                    Order = 1, 
                    Source = EventValueSource.Static, 
                    Value = OWNER_ADDRESS}
            };

            var result = await contractQueryEventHandler.HandleAsync(decodedEvent);

            Verify(result);
        }

        [Fact]
        public async Task CanBeFromEventAddress()
        {
            queryConfig.Parameters = new[]
            {
                new ContractQueryParameter{
                    Order = 1, 
                    Source = EventValueSource.EventAddress}
            };

            decodedEvent.Log.Address  = OWNER_ADDRESS;

            var result = await contractQueryEventHandler.HandleAsync(decodedEvent);

            Verify(result);
        }

        [Fact]
        public async Task CanBeFromEventState()
        {
            const string EVENT_STATE_KEY = "OwnerAddress";

            queryConfig.Parameters = new[]
            {
                new ContractQueryParameter{
                    Order = 1, 
                    Source = EventValueSource.EventState, 
                    EventStateName = EVENT_STATE_KEY}
            };

            decodedEvent.State[EVENT_STATE_KEY]  = OWNER_ADDRESS;

            var result = await contractQueryEventHandler.HandleAsync(decodedEvent);

            Verify(result);
        }

        private void Verify(bool result)
        {
            Assert.True(result);
            Assert.Equal(OWNER_ADDRESS, ActualFunctionInputArgs[0]);
            Assert.Equal(FAKE_QUERY_RESULT, decodedEvent.State[EVENT_STATE_QUERY_OUTPUT_NAME]);
        }
    }
}
