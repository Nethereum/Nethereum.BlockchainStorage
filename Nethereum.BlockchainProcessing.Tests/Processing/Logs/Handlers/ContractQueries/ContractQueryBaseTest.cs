using Moq;
using Nethereum.BlockchainProcessing.BlockchainProxy;
using Nethereum.BlockchainProcessing.Processing.Logs;
using Nethereum.BlockchainProcessing.Processing.Logs.Handling;
using System;

namespace Nethereum.BlockchainProcessing.Tests.Processing.Logs.HandlerTests.ContractQueryTests
{
    public abstract class ContractQueryBaseTest
    {
        protected const string CONTRACT_ADDRESS = "0x78c1301520edff0bb14314c64987a71fa5efa407";
        protected const string OWNER_ADDRESS = "0x12c1301520edff0bb14314c64987a71fa5efa407";

        protected abstract object FAKE_QUERY_RESULT {get; }

        public static class SHA3_FUNCTION_SIGNATURES
        {
            public const string NAME = "06fdde03";
            public const string BALANCE_OF = "70a08231";
            public const string APPROVE = "095ea7b3";
        }
        
        public struct QueryArgs
        {
            public string ContractAddress {get;set;}
            public string Abi {get;set;}
            public string FunctionSignature {get;set;}
            public object[] FunctionInputValues {get;set;}
        }

        public static IContractQuery MockContractQuery(object valueToReturn, Action<QueryArgs> callBack = null)
        {
            var _mock = new Mock<IContractQuery>();

            _mock.Setup(p => p.Query(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<object[]>()))
            .Callback<string, string, string, object[]>((p1, p2, p3, p4) => callBack?.Invoke(new QueryArgs{
                ContractAddress = p1, Abi = p2, FunctionSignature = p3, FunctionInputValues = p4
            }))
            .ReturnsAsync(valueToReturn);

            return _mock.Object;
        }

        protected DecodedEvent decodedEvent;
        protected ContractQueryEventHandler contractQueryEventHandler;
        protected ContractQueryConfiguration queryConfig;
        protected EventSubscriptionStateDto subscriptionState;
        protected QueryArgs actualQueryArgs;

        public ContractQueryBaseTest(ContractQueryConfiguration queryConfig)
        {
            this.queryConfig = queryConfig;

            decodedEvent = DecodedEvent.Empty();

            var mockContractQuery =  MockContractQuery(FAKE_QUERY_RESULT, (actualArgs) => actualQueryArgs = actualArgs);

            subscriptionState = new EventSubscriptionStateDto();
            contractQueryEventHandler = new ContractQueryEventHandler(mockContractQuery, subscriptionState, queryConfig);
        }
    }
}
