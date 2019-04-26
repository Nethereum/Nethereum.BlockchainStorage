using Nethereum.ABI.FunctionEncoding;
using Nethereum.ABI.Model;
using Nethereum.BlockchainProcessing.Processing.Logs;
using Nethereum.BlockchainProcessing.Processing.Logs.Configuration;
using Nethereum.BlockchainProcessing.Processing.Logs.Handling;
using Nethereum.BlockchainProcessing.Processing.Logs.Handling.Handlers;
using System.Threading.Tasks;
using Xunit;

namespace Nethereum.BlockchainProcessing.Tests.Processing.Logs.HandlerTests.ContractQueryTests
{

    public class ContractQueryAddressTests : ContractQueryBaseTest
    {
        private string ActualContractAddress => actualQueryArgs.ContractAddress;
        protected override object FAKE_QUERY_RESULT => "DW";

        public ContractQueryAddressTests():base(new ContractQueryConfiguration
            {
                Contract = new SubscriberContractDto{Abi = TestData.Contracts.StandardContract.Abi },
                Query = new ContractQueryDto{ FunctionSignature = SHA3_FUNCTION_SIGNATURES.NAME,
                EventStateOutputName = "EIRC20_Name" }
            }){}

        [Fact]
        public async Task CanBeStatic()
        {
            const string CONTRACT_TO_QUERY = CONTRACT_ADDRESS;

            queryConfig.Query.ContractAddressSource = ContractAddressSource.Static;
            queryConfig.Query.ContractAddress = CONTRACT_TO_QUERY;

            await ExecuteAndVerify(CONTRACT_TO_QUERY);
        }

        [Fact]
        public async Task CanBeEventAddress()
        {
            const string CONTRACT_TO_QUERY = CONTRACT_ADDRESS;

            queryConfig.Query.ContractAddressSource = ContractAddressSource.EventAddress;
            decodedEvent.Log.Address = CONTRACT_TO_QUERY;

            await ExecuteAndVerify(CONTRACT_TO_QUERY);
        }

        [Fact]
        public async Task CanBeFromEventParameter()
        {
            const string CONTRACT_TO_QUERY = CONTRACT_ADDRESS;
            const int PARAMETER_NUMBER = 1;

            queryConfig.Query.ContractAddressSource = ContractAddressSource.EventParameter;
            queryConfig.Query.ContractAddressParameterNumber = PARAMETER_NUMBER;
            decodedEvent.Event.Add(new ParameterOutput{
                Result = CONTRACT_TO_QUERY, 
                Parameter = new Parameter("address", PARAMETER_NUMBER)});

            await ExecuteAndVerify(CONTRACT_TO_QUERY);
        }

        [Fact]
        public async Task CanBeFromEventState()
        {
            const string CONTRACT_TO_QUERY = CONTRACT_ADDRESS;
            const string STATE_VARIABLE_NAME = "ContractAddressToQuery";

            queryConfig.Query.ContractAddressSource = ContractAddressSource.EventState;
            queryConfig.Query.ContractAddressStateVariableName = STATE_VARIABLE_NAME;
            decodedEvent.State[STATE_VARIABLE_NAME] = CONTRACT_TO_QUERY;
        
            await ExecuteAndVerify(CONTRACT_TO_QUERY);
        }

        [Fact]
        public async Task CanBeTransactionFromAddress()
        {
            const string CONTRACT_TO_QUERY = CONTRACT_ADDRESS;
            const string STATE_VARIABLE_NAME = "ContractAddressToQuery";

            queryConfig.Query.ContractAddressSource = ContractAddressSource.TransactionFrom;
            queryConfig.Query.ContractAddressStateVariableName = STATE_VARIABLE_NAME;
            decodedEvent.Transaction = new RPC.Eth.DTOs.Transaction{From = CONTRACT_TO_QUERY };
        
            await ExecuteAndVerify(CONTRACT_TO_QUERY);
        }

        [Fact]
        public async Task CanBeTransactionToAddress()
        {
            const string CONTRACT_TO_QUERY = CONTRACT_ADDRESS;
            const string STATE_VARIABLE_NAME = "ContractAddressToQuery";

            queryConfig.Query.ContractAddressSource = ContractAddressSource.TransactionTo;
            queryConfig.Query.ContractAddressStateVariableName = STATE_VARIABLE_NAME;
            decodedEvent.Transaction = new RPC.Eth.DTOs.Transaction{To = CONTRACT_TO_QUERY };
        
            await ExecuteAndVerify(CONTRACT_TO_QUERY);
        }

        private async Task ExecuteAndVerify(string CONTRACT_TO_QUERY)
        {
            Assert.True(await contractQueryEventHandler.HandleAsync(decodedEvent));
            Assert.Equal(FAKE_QUERY_RESULT, decodedEvent.State["EIRC20_Name"]);
            Assert.Equal(CONTRACT_TO_QUERY, ActualContractAddress);
        }


    }
}
