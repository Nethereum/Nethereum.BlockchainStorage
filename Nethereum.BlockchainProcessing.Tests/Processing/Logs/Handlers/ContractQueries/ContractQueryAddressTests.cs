using Nethereum.ABI.FunctionEncoding;
using Nethereum.ABI.Model;
using Nethereum.BlockchainProcessing.Processing.Logs;
using Nethereum.BlockchainProcessing.Processing.Logs.Handling;
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
                ContractABI = TestData.Contracts.StandardContract.Abi,
                FunctionSignature = SHA3_FUNCTION_SIGNATURES.NAME,
                EventStateOutputName = "EIRC20_Name"
            }){}

        [Fact]
        public async Task CanBeStatic()
        {
            const string CONTRACT_TO_QUERY = CONTRACT_ADDRESS;

            queryConfig.ContractAddressSource = ContractAddressSource.Static;
            queryConfig.ContractAddress = CONTRACT_TO_QUERY;

            await ExecuteAndVerify(CONTRACT_TO_QUERY);
        }

        [Fact]
        public async Task CanBeEventAddress()
        {
            const string CONTRACT_TO_QUERY = CONTRACT_ADDRESS;

            queryConfig.ContractAddressSource = ContractAddressSource.EventAddress;
            decodedEvent.Log.Address = CONTRACT_TO_QUERY;

            await ExecuteAndVerify(CONTRACT_TO_QUERY);
        }

        [Fact]
        public async Task CanBeFromEventParameter()
        {
            const string CONTRACT_TO_QUERY = CONTRACT_ADDRESS;
            const int PARAMETER_NUMBER = 1;

            queryConfig.ContractAddressSource = ContractAddressSource.EventParameter;
            queryConfig.ContractAddressParameterNumber = PARAMETER_NUMBER;
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

            queryConfig.ContractAddressSource = ContractAddressSource.EventState;
            queryConfig.ContractAddressStateVariableName = STATE_VARIABLE_NAME;
            decodedEvent.State[STATE_VARIABLE_NAME] = CONTRACT_TO_QUERY;
        
            await ExecuteAndVerify(CONTRACT_TO_QUERY);
        }

        [Fact]
        public async Task CanBeTransactionFromAddress()
        {
            const string CONTRACT_TO_QUERY = CONTRACT_ADDRESS;
            const string STATE_VARIABLE_NAME = "ContractAddressToQuery";

            queryConfig.ContractAddressSource = ContractAddressSource.TransactionFrom;
            queryConfig.ContractAddressStateVariableName = STATE_VARIABLE_NAME;
            decodedEvent.Transaction = new RPC.Eth.DTOs.Transaction{From = CONTRACT_TO_QUERY };
        
            await ExecuteAndVerify(CONTRACT_TO_QUERY);
        }

        [Fact]
        public async Task CanBeTransactionToAddress()
        {
            const string CONTRACT_TO_QUERY = CONTRACT_ADDRESS;
            const string STATE_VARIABLE_NAME = "ContractAddressToQuery";

            queryConfig.ContractAddressSource = ContractAddressSource.TransactionTo;
            queryConfig.ContractAddressStateVariableName = STATE_VARIABLE_NAME;
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
