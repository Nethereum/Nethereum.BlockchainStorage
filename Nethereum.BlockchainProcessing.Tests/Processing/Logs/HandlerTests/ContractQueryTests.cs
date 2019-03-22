using Moq;
using Nethereum.ABI.FunctionEncoding;
using Nethereum.ABI.Model;
using Nethereum.BlockchainProcessing.BlockchainProxy;
using Nethereum.BlockchainProcessing.Processing.Logs;
using Nethereum.BlockchainProcessing.Processing.Logs.Handling;
using System.Numerics;
using System.Threading.Tasks;
using Xunit;

namespace Nethereum.BlockchainProcessing.Tests.Processing.Logs
{
    public class ContractQueryTests
    {
        public const string CONTRACT_ADDRESS = "0x78c1301520edff0bb14314c64987a71fa5efa407";
        public const string OWNER_ADDRESS = "0x12c1301520edff0bb14314c64987a71fa5efa407";
        public const string FUNCTION_SIGNATURE_FOR_NAME = "06fdde03";
        public const string FUNCTION_SIGNATURE_FOR_BALANCE_OF = "70a08231";

        Mock<IContractQuery> _mockProxy = new Mock<IContractQuery>();

        [Fact]
        public async Task ContractAddressCanBeStatic()
        {
            var queryConfig =  new ContractQueryConfiguration
                {
                    ContractAddressSource = ContractAddressSource.Static,
                    ContractAddress = CONTRACT_ADDRESS,
                    ContractABI = TestData.Contracts.StandardContract.Abi,
                    FunctionSignature = FUNCTION_SIGNATURE_FOR_NAME,
                    MetaDataOutputName = "EIRC20_Name"
                };

            var state = new EventSubscriptionStateDto();

            var contractQuery = new ContractQuery(_mockProxy.Object, state, queryConfig);

            _mockProxy
                .Setup(p => p.Query(queryConfig.ContractAddress, queryConfig.ContractABI, queryConfig.FunctionSignature, null))
                .ReturnsAsync("DW");

            var decodedEventLog = DecodedEvent.Empty();

            var result = await contractQuery.HandleAsync(decodedEventLog);

            Assert.True(result);
            Assert.Equal("DW", decodedEventLog.Metadata["EIRC20_Name"]);
        }

        [Fact]
        public async Task ContractAddressCanBeEventAddress()
        {
            var queryConfig =  new ContractQueryConfiguration
                {
                    ContractAddressSource = ContractAddressSource.EventAddress,
                    ContractABI = TestData.Contracts.StandardContract.Abi,
                    FunctionSignature = FUNCTION_SIGNATURE_FOR_NAME,
                    MetaDataOutputName = "EIRC20_Name"
                };

            var state = new EventSubscriptionStateDto();

            var contractQuery = new ContractQuery(_mockProxy.Object, state, queryConfig);

            _mockProxy
                .Setup(p => p.Query(CONTRACT_ADDRESS, queryConfig.ContractABI, queryConfig.FunctionSignature, null))
                .ReturnsAsync("DW");

            var decodedEventLog = DecodedEvent.Empty();
            decodedEventLog.Log.Log.Address = CONTRACT_ADDRESS;

            var result = await contractQuery.HandleAsync(decodedEventLog);

            Assert.True(result);
            Assert.Equal("DW", decodedEventLog.Metadata["EIRC20_Name"]);
        }

        [Fact]
        public async Task ContractAddressCanBeFromEventParameter()
        {
            var queryConfig =  new ContractQueryConfiguration
                {
                    ContractAddressSource = ContractAddressSource.EventParameter,
                    ContractAddressParameterNumber = 1,
                    ContractABI = TestData.Contracts.StandardContract.Abi,
                    FunctionSignature = FUNCTION_SIGNATURE_FOR_NAME,
                    MetaDataOutputName = "EIRC20_Name"
                };

            var state = new EventSubscriptionStateDto();

            var contractQuery = new ContractQuery(_mockProxy.Object, state, queryConfig);

            _mockProxy
                .Setup(p => p.Query(CONTRACT_ADDRESS, queryConfig.ContractABI, queryConfig.FunctionSignature, null))
                .ReturnsAsync("DW");

            var decodedEventLog = DecodedEvent.Empty();
            decodedEventLog.Log.Event.Add(new ParameterOutput{Result = CONTRACT_ADDRESS, Parameter = new Parameter("address", 1)});

            var result = await contractQuery.HandleAsync(decodedEventLog);

            Assert.True(result);
            Assert.Equal("DW", decodedEventLog.Metadata["EIRC20_Name"]);
        }

        [Fact]
        public async Task ContractAddressCanBeFromEventMetadata()
        {
            var queryConfig =  new ContractQueryConfiguration
                {
                    ContractAddressSource = ContractAddressSource.EventMetaData,
                    ContractAddressMetadataVariableName = "ContractAddressToQuery",
                    ContractABI = TestData.Contracts.StandardContract.Abi,
                    FunctionSignature = FUNCTION_SIGNATURE_FOR_NAME,
                    MetaDataOutputName = "EIRC20_Name"
                };

            var state = new EventSubscriptionStateDto();

            var contractQuery = new ContractQuery(_mockProxy.Object, state, queryConfig);

            _mockProxy
                .Setup(p => p.Query(CONTRACT_ADDRESS, queryConfig.ContractABI, queryConfig.FunctionSignature, null))
                .ReturnsAsync("DW");

            var decodedEventLog = DecodedEvent.Empty();
            decodedEventLog.Metadata["ContractAddressToQuery"] = CONTRACT_ADDRESS;

            var result = await contractQuery.HandleAsync(decodedEventLog);

            Assert.True(result);
            Assert.Equal("DW", decodedEventLog.Metadata["EIRC20_Name"]);
        }

        [Fact]
        public async Task CanOutputResultToEventSubscriptionState()
        {
            var queryConfig =  new ContractQueryConfiguration
                {
                    ContractAddressSource = ContractAddressSource.Static,
                    ContractAddress = CONTRACT_ADDRESS,
                    ContractABI = TestData.Contracts.StandardContract.Abi,
                    FunctionSignature = FUNCTION_SIGNATURE_FOR_NAME,
                    EventSuscriptionStateVariableName = "EIRC20_Name"
                };

            var state = new EventSubscriptionStateDto();

            var contractQuery = new ContractQuery(_mockProxy.Object, state, queryConfig);

            _mockProxy
                .Setup(p => p.Query(queryConfig.ContractAddress, queryConfig.ContractABI, queryConfig.FunctionSignature, null))
                .ReturnsAsync("DW");

            var decodedEventLog = DecodedEvent.Empty();

            var result = await contractQuery.HandleAsync(decodedEventLog);

            Assert.True(result);
            Assert.Equal("DW", state.Get("EIRC20_Name"));
        }

        [Fact]
        public async Task CanOutputResultToEventMetaData()
        {
            var queryConfig =  new ContractQueryConfiguration
                {
                    ContractAddressSource = ContractAddressSource.Static,
                    ContractAddress = CONTRACT_ADDRESS,
                    ContractABI = TestData.Contracts.StandardContract.Abi,
                    FunctionSignature = FUNCTION_SIGNATURE_FOR_NAME,
                    MetaDataOutputName = "EIRC20_Name"
                };

            var state = new EventSubscriptionStateDto();

            var contractQuery = new ContractQuery(_mockProxy.Object, state, queryConfig);

            _mockProxy
                .Setup(p => p.Query(queryConfig.ContractAddress, queryConfig.ContractABI, queryConfig.FunctionSignature, null))
                .ReturnsAsync("DW");

            var decodedEventLog = DecodedEvent.Empty();

            var result = await contractQuery.HandleAsync(decodedEventLog);

            Assert.True(result);
            Assert.Equal("DW", decodedEventLog.Metadata["EIRC20_Name"]);
        }

        [Fact]
        public async Task CanOutputResultToEventMetaDataAndSubscriptionState()
        {
            var queryConfig =  new ContractQueryConfiguration
                {
                    ContractAddressSource = ContractAddressSource.Static,
                    ContractAddress = CONTRACT_ADDRESS,
                    ContractABI = TestData.Contracts.StandardContract.Abi,
                    FunctionSignature = FUNCTION_SIGNATURE_FOR_NAME,
                    MetaDataOutputName = "EIRC20_Name",
                    EventSuscriptionStateVariableName = "EIRC20_Name"
                };

            var state = new EventSubscriptionStateDto();

            var contractQuery = new ContractQuery(_mockProxy.Object, state, queryConfig);

            _mockProxy
                .Setup(p => p.Query(queryConfig.ContractAddress, queryConfig.ContractABI, queryConfig.FunctionSignature, null))
                .ReturnsAsync("DW");

            var decodedEventLog = DecodedEvent.Empty();

            var result = await contractQuery.HandleAsync(decodedEventLog);

            Assert.True(result);
            Assert.Equal("DW", decodedEventLog.Metadata["EIRC20_Name"]);
            Assert.Equal("DW", state.Get("EIRC20_Name"));
        }

        [Fact]
        public async Task FunctionInputValueCanBeFromEventParameter()
        {
            var queryConfig =  new ContractQueryConfiguration
                {
                    ContractAddressSource = ContractAddressSource.Static,
                    ContractAddress = CONTRACT_ADDRESS,
                    ContractABI = TestData.Contracts.StandardContract.Abi,
                    FunctionSignature = FUNCTION_SIGNATURE_FOR_BALANCE_OF,
                    MetaDataOutputName = "EIRC20_Balance",
                    Parameters = new[]
                    {
                        new ContractQueryParameter{Order = 1, Source = EventValueSource.EventParameters, EventParameterNumber = 1}
                    }
                };

            var state = new EventSubscriptionStateDto();

            var contractQuery = new ContractQuery(_mockProxy.Object, state, queryConfig);

            object[] actualFunctionInputs = null;

            _mockProxy
                .Setup(p => p.Query(queryConfig.ContractAddress, queryConfig.ContractABI, queryConfig.FunctionSignature, It.IsAny<object[]>()))
                .Callback<string, string, string, object[]>((p1, p2, p3, functionInputs) => actualFunctionInputs = functionInputs)
                .ReturnsAsync(BigInteger.One);

            var decodedEventLog = DecodedEvent.Empty();
            decodedEventLog.Log.Event.Add(new ParameterOutput{Result = OWNER_ADDRESS, Parameter = new Parameter("address", 1) });

            var result = await contractQuery.HandleAsync(decodedEventLog);

            Assert.True(result);
            Assert.Equal(OWNER_ADDRESS, actualFunctionInputs[0]);
            Assert.Equal(BigInteger.One, decodedEventLog.Metadata["EIRC20_Balance"]);
        }

        [Fact]
        public async Task FunctionInputValueCanBeAStaticValue()
        {
            var queryConfig =  new ContractQueryConfiguration
                {
                    ContractAddressSource = ContractAddressSource.Static,
                    ContractAddress = CONTRACT_ADDRESS,
                    ContractABI = TestData.Contracts.StandardContract.Abi,
                    FunctionSignature = FUNCTION_SIGNATURE_FOR_BALANCE_OF,
                    MetaDataOutputName = "EIRC20_Balance",
                    Parameters = new[]
                    {
                        new ContractQueryParameter{Order = 1, Source = EventValueSource.Static, Value = OWNER_ADDRESS}
                    }
                };

            var state = new EventSubscriptionStateDto();

            var contractQuery = new ContractQuery(_mockProxy.Object, state, queryConfig);

            object[] actualFunctionInputs = null;

            _mockProxy
                .Setup(p => p.Query(queryConfig.ContractAddress, queryConfig.ContractABI, queryConfig.FunctionSignature, It.IsAny<object[]>()))
                .Callback<string, string, string, object[]>((p1, p2, p3, functionInputs) => actualFunctionInputs = functionInputs)
                .ReturnsAsync(BigInteger.One);

            var decodedEventLog = DecodedEvent.Empty();

            var result = await contractQuery.HandleAsync(decodedEventLog);

            Assert.True(result);
            Assert.Equal(OWNER_ADDRESS, actualFunctionInputs[0]);
            Assert.Equal(BigInteger.One, decodedEventLog.Metadata["EIRC20_Balance"]);
        }

        [Fact]
        public async Task FunctionInputValueCanBeFromEventMetadata()
        {
            var queryConfig =  new ContractQueryConfiguration
                {
                    ContractAddressSource = ContractAddressSource.Static,
                    ContractAddress = CONTRACT_ADDRESS,
                    ContractABI = TestData.Contracts.StandardContract.Abi,
                    FunctionSignature = FUNCTION_SIGNATURE_FOR_BALANCE_OF,
                    MetaDataOutputName = "EIRC20_Balance",
                    Parameters = new[]
                    {
                        new ContractQueryParameter{Order = 1, Source = EventValueSource.EventMetaData, EventMetadataName = "OwnerAddress"}
                    }
                };

            var state = new EventSubscriptionStateDto();

            var contractQuery = new ContractQuery(_mockProxy.Object, state, queryConfig);

            object[] actualFunctionInputs = null;

            _mockProxy
                .Setup(p => p.Query(queryConfig.ContractAddress, queryConfig.ContractABI, queryConfig.FunctionSignature, It.IsAny<object[]>()))
                .Callback<string, string, string, object[]>((p1, p2, p3, functionInputs) => actualFunctionInputs = functionInputs)
                .ReturnsAsync(BigInteger.One);

            var decodedEventLog = DecodedEvent.Empty();
            decodedEventLog.Metadata["OwnerAddress"]  = OWNER_ADDRESS;

            var result = await contractQuery.HandleAsync(decodedEventLog);

            Assert.True(result);
            Assert.Equal(OWNER_ADDRESS, actualFunctionInputs[0]);
            Assert.Equal(BigInteger.One, decodedEventLog.Metadata["EIRC20_Balance"]);
        }

    }
}
