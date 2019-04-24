using Nethereum.BlockchainProcessing.Processing.Logs;
using Nethereum.BlockchainProcessing.Processing.Logs.Configuration;
using Nethereum.BlockchainProcessing.Processing.Logs.Matching;
using Nethereum.RPC.Eth.DTOs;
using Xunit;

namespace Nethereum.BlockchainProcessing.Tests.Processing.Logs
{
    public class GenericEventSubscriptionTests
    {
        [Fact]
        public void MatchesEventSignature()
        {
            var eventSubscription = new EventSubscription<TestData.Contracts.StandardContract.TransferEvent>();

            var transferLog = TestData.Contracts.StandardContract.SampleTransferLog();
            var notATransferLog = new FilterLog();

            Assert.True(eventSubscription.IsLogForEvent(transferLog));
            Assert.False(eventSubscription.IsLogForEvent(notATransferLog));
        }

        [Fact]
        public void MatchesEventSignatureAndContractAddress()
        {
            var contractAddresses = new[] { "0x243e72b69141f6af525a9a5fd939668ee9f2b354" };
            var eventSubscription = new EventSubscription<TestData.Contracts.StandardContract.TransferEvent>(
                contractAddressesToMatch: contractAddresses);

            var transferLog = TestData.Contracts.StandardContract.SampleTransferLog();
            var transferLogForAnotherContract = TestData.Contracts.StandardContract.SampleTransferLog();
            transferLogForAnotherContract.Address = "0x343e72b69141f6af525a9a5fd939668ee9f2b354";

            Assert.True(eventSubscription.IsLogForEvent(transferLog));
            Assert.False(eventSubscription.IsLogForEvent(transferLogForAnotherContract));
        }

        [Fact]
        public void MatchesEventSignatureAndParameterCondition()
        {
            var fromCondition = ParameterCondition.Create(
                1, ParameterConditionOperator.Equals, "0x12890d2cce102216644c59dae5baed380d84830c");

            var eventSubscription = new EventSubscription<TestData.Contracts.StandardContract.TransferEvent>(
                parameterConditions: new IParameterCondition[]{ fromCondition} );

            var transferLog = TestData.Contracts.StandardContract.SampleTransferLog();
            var transferWithDifferentFromAddress = TestData.Contracts.StandardContract.SampleTransferLog();
            transferWithDifferentFromAddress.Topics[1] = "0x00000000000000000000000013f022d72158410433cbd66f5dd8bf6d2d129924";

            Assert.True(eventSubscription.IsLogForEvent(transferLog));
            Assert.False(eventSubscription.IsLogForEvent(transferWithDifferentFromAddress));
        }
    }
}
