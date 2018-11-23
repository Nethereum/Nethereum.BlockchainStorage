using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.BlockchainProcessing.Processing;
using Nethereum.BlockchainProcessing.Processing.Logs;
using Nethereum.Contracts;
using Nethereum.Hex.HexConvertors.Extensions;
using System;
using System.Linq;
using System.Numerics;
using Xunit;

namespace Nethereum.BlockchainProcessing.Tests.Processing.Logs
{
    public class NewFilterInputBuilderTests
    {
        [Event("Transfer")]
        public class TransferEvent
        {
            [Parameter("address", "_from", 1, true)]
            public string From { get; set; }

            [Parameter("address", "_to", 2, true)] 
            public string To { get; set; }

            [Parameter("uint256", "_value", 3, true)]
            public BigInteger Value { get; set; }
        }

        [Event("TransferEvent_WithEmptyParameterNames")]
        public class TransferEvent_WithEmptyParameterNames
        {
            [Parameter("address", "", 1, true)]
            public string From { get; set; }

            [Parameter("address", null, 2, true)] 
            public string To { get; set; }

            [Parameter("uint256", " ", 3, true)]
            public BigInteger Value { get; set; }
        }

        [Fact]
        public void Assigns_Event_Signature_To_Topic0()
        {
            var filter = new NewFilterInputBuilder<TransferEvent>().Build();

            var eventAbi = ABITypedRegistry.GetEvent<TransferEvent>();

            Assert.Equal(eventAbi.Sha3Signature.EnsureHexPrefix(), filter.Topics.FirstOrDefault());

            Assert.False(filter.IsTopicFiltered(1));
            Assert.False(filter.IsTopicFiltered(2));
            Assert.False(filter.IsTopicFiltered(3));
        }

        [Fact]
        public void Can_Assign_To_Topic1()
        {
            var from = "0xc14934679e71ef4d18b6ae927fe2b953c7fd9b91";

            var filter = new NewFilterInputBuilder<TransferEvent>()
                .AddTopic((t) => t.From, from)
                .Build();

            Assert.Equal("0x000000000000000000000000c14934679e71ef4d18b6ae927fe2b953c7fd9b91", 
                filter.GetFirstTopicValueAsString(1));

            Assert.False(filter.IsTopicFiltered(2));
            Assert.False(filter.IsTopicFiltered(3));
        }

        [Fact]
        public void Can_Assign_Many_Values_To_A_Topic()
        {
            var address1 = "0xc14934679e71ef4d18b6ae927fe2b953c7fd9b91";
            var address2 = "0xc24934679e71ef4d18b6ae927fe2b953c7fd9b91";

            var filter = new NewFilterInputBuilder<TransferEvent>()
                .AddTopic((t) => t.From, address1)
                .AddTopic((t) => t.From, address2)
                .Build();

            var topicValues = filter.GetTopicValues(1);

            Assert.Equal("0x000000000000000000000000c14934679e71ef4d18b6ae927fe2b953c7fd9b91", 
                topicValues[0].ToString());

            Assert.Equal("0x000000000000000000000000c24934679e71ef4d18b6ae927fe2b953c7fd9b91", 
                topicValues[1].ToString());
        }

        [Fact]
        public void Can_Assign_Many_Values_To_A_Topic_At_Once()
        {
            var address1 = "0xc14934679e71ef4d18b6ae927fe2b953c7fd9b91";
            var address2 = "0xc24934679e71ef4d18b6ae927fe2b953c7fd9b91";

            var filter = new NewFilterInputBuilder<TransferEvent>()
                .AddTopic((t) => t.From, new []{address1, address2})
                .Build();

            var topicValues = filter.GetTopicValues(1);

            Assert.Equal("0x000000000000000000000000c14934679e71ef4d18b6ae927fe2b953c7fd9b91", 
                topicValues[0].ToString());

            Assert.Equal("0x000000000000000000000000c24934679e71ef4d18b6ae927fe2b953c7fd9b91", 
                topicValues[1].ToString());
        }

        [Fact]
        public void Can_Assign_To_Topic2()
        {
            var to = "0xc14934679e71ef4d18b6ae927fe2b953c7fd9b91";

            var filter = new NewFilterInputBuilder<TransferEvent>()
                .AddTopic(template => template.To, to)
                .Build();

            Assert.False(filter.IsTopicFiltered(1));
            Assert.Equal("0x000000000000000000000000c14934679e71ef4d18b6ae927fe2b953c7fd9b91", 
                filter.GetFirstTopicValueAsString(2));
            Assert.False(filter.IsTopicFiltered(3));
        }

        [Fact]
        public void Can_Assign_To_Topic3()
        {
            var value = BigInteger.One;

            var filter = new NewFilterInputBuilder<TransferEvent>()
                .AddTopic(template => template.Value, value)
                .Build();

            Assert.False(filter.IsTopicFiltered(1));
            Assert.False(filter.IsTopicFiltered(2));
            Assert.Equal("0x0000000000000000000000000000000000000000000000000000000000000001", 
                filter.GetFirstTopicValueAsString(3));
        }


        [Fact]
        public void Can_Assign_To_Multiple_Topics()
        {
            var from = "0xc14934679e71ef4d18b6ae927fe2b953c7fd9b91";
            var to = "0xc14934679e71ef4d18b6ae927fe2b953c7fd9b92";
            var value = BigInteger.One;

            var filter = new NewFilterInputBuilder<TransferEvent>()
                .AddTopic(template => template.From, from)
                .AddTopic(template => template.To,  to)
                .AddTopic(template => template.Value, value)
                .Build();

            Assert.Equal("0x000000000000000000000000c14934679e71ef4d18b6ae927fe2b953c7fd9b91", 
                filter.GetFirstTopicValueAsString(1));

            Assert.Equal("0x000000000000000000000000c14934679e71ef4d18b6ae927fe2b953c7fd9b92", 
                filter.GetFirstTopicValueAsString(2));

            Assert.Equal("0x0000000000000000000000000000000000000000000000000000000000000001", 
                filter.GetFirstTopicValueAsString(3));
        }

        [Fact]
        public void When_Parameter_Name_Is_Empty_Uses_Order_To_Find_Topic()
        {
            var from = "0xc14934679e71ef4d18b6ae927fe2b953c7fd9b91";
            var to = "0xc14934679e71ef4d18b6ae927fe2b953c7fd9b92";
            var value = BigInteger.One;

            var filter = new NewFilterInputBuilder<TransferEvent_WithEmptyParameterNames>()
                .AddTopic(template => template.From, from)
                .AddTopic(template => template.To,  to)
                .AddTopic(template => template.Value, value)
                .Build();

            Assert.Equal("0x000000000000000000000000c14934679e71ef4d18b6ae927fe2b953c7fd9b91", 
                filter.GetFirstTopicValueAsString(1));

            Assert.Equal("0x000000000000000000000000c14934679e71ef4d18b6ae927fe2b953c7fd9b92", 
                filter.GetFirstTopicValueAsString(2));

            Assert.Equal("0x0000000000000000000000000000000000000000000000000000000000000001", 
                filter.GetFirstTopicValueAsString(3));
        }

        [Fact]
        public void Assigns_Specified_Contract_Addresses()
        {
            var ContractAddresses = new []
            {
                "0xC03cDD393C89D169bd4877d58f0554f320f21037",
                "0xD03cDD393C89D169bd4877d58f0554f320f21037"
            };

            var filter = new NewFilterInputBuilder<TransferEvent>().Build(ContractAddresses);

            Assert.True(filter.Address.SequenceEqual(ContractAddresses));
        }

        [Fact]
        public void Assigns_Specified_Contract_Address()
        {
            var contractAddress = 
                "0xC03cDD393C89D169bd4877d58f0554f320f21037";

            var filter = new NewFilterInputBuilder<TransferEvent>().Build(contractAddress);

            Assert.Single(filter.Address, contractAddress);
        }

        [Fact]
        public void Assigns_Specified_Block_Numbers()
        {
            var range = new BlockRange(15, 25);

            var filter = new NewFilterInputBuilder<TransferEvent>().Build(blockRange: range);

            Assert.Equal(range.From, filter.FromBlock.BlockNumber.Value);
            Assert.Equal(range.To, filter.ToBlock.BlockNumber.Value);
        }
    }
}