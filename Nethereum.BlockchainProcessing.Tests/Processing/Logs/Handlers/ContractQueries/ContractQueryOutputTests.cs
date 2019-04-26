using Nethereum.BlockchainProcessing.Processing.Logs;
using Nethereum.BlockchainProcessing.Processing.Logs.Configuration;
using Nethereum.BlockchainProcessing.Processing.Logs.Handling;
using Nethereum.BlockchainProcessing.Processing.Logs.Handling.Handlers;
using System.Threading.Tasks;
using Xunit;

namespace Nethereum.BlockchainProcessing.Tests.Processing.Logs.HandlerTests.ContractQueryTests
{
    public class ContractQueryOutputTests : ContractQueryBaseTest
    {
        protected override object FAKE_QUERY_RESULT => "DW";

        public ContractQueryOutputTests():base(new ContractQueryConfiguration
            {
               Contract = new SubscriberContractDto{ Abi = TestData.Contracts.StandardContract.Abi },
               Query = new ContractQueryDto{ FunctionSignature = SHA3_FUNCTION_SIGNATURES.NAME,
                ContractAddressSource = ContractAddressSource.Static,
                ContractAddress = CONTRACT_ADDRESS}
        })
        {
        }

        [Fact]
        public async Task CanOutputResultToEventSubscriptionState()
        {
            const string STATE_NAME = "EIRC20_Name";
            queryConfig.Query.SubscriptionStateOutputName = STATE_NAME;

            Assert.True(await contractQueryEventHandler.HandleAsync(decodedEvent));

            Assert.Equal(FAKE_QUERY_RESULT, subscriptionState.Get(STATE_NAME));
        }

        [Fact]
        public async Task CanOutputResultToEventState()
        {
            const string STATE_NAME = "EIRC20_Name";
            queryConfig.Query.EventStateOutputName = STATE_NAME;

            Assert.True(await contractQueryEventHandler.HandleAsync(decodedEvent));

            Assert.Equal(FAKE_QUERY_RESULT, decodedEvent.State[STATE_NAME]);
        }

        [Fact]
        public async Task CanOutputResultToEventStateAndSubscriptionState()
        {
            const string SUBSCRIPTION_STATE_NAME = "EIRC20_Name_1";
            const string EVENT_STATE_NAME = "EIRC20_Name_2";

            queryConfig.Query.EventStateOutputName = EVENT_STATE_NAME;
            queryConfig.Query.SubscriptionStateOutputName = SUBSCRIPTION_STATE_NAME;

            Assert.True(await contractQueryEventHandler.HandleAsync(decodedEvent));

            Assert.Equal(FAKE_QUERY_RESULT, subscriptionState.Get(SUBSCRIPTION_STATE_NAME));
            Assert.Equal(FAKE_QUERY_RESULT, decodedEvent.State[EVENT_STATE_NAME]);
        }
    }
}
