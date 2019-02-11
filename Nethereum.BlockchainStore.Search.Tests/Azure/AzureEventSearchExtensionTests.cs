using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using Microsoft.Azure.Search.Models;
using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.BlockchainStore.Search.Azure;
using Nethereum.Contracts;
using Nethereum.Hex.HexTypes;
using Nethereum.RPC.Eth.DTOs;
using Xunit;

namespace Nethereum.BlockchainStore.Search.Tests.Azure
{
    public class AzureEventSearchExtensionTests
    {
        [Fact]
        public void ToAzureDataType()
        {
            //mapped field types
            Assert.Equal(DataType.String, typeof(string).ToAzureDataType());
            Assert.Equal(DataType.String, typeof(BigInteger).ToAzureDataType());
            Assert.Equal(DataType.String, typeof(HexBigInteger).ToAzureDataType());
            Assert.Equal(DataType.Boolean, typeof(bool).ToAzureDataType());

            //unmapped - default field type is string
            Assert.Equal(DataType.String, typeof(DateTime).ToAzureDataType());
            Assert.Equal(DataType.String, typeof(int).ToAzureDataType());
            Assert.Equal(DataType.String, typeof(long).ToAzureDataType());
        }

        [Fact]
        public void ToAzureFieldValue()
        {
            Assert.Equal("hello", "hello".ToAzureFieldValue());
            Assert.Equal(true, true.ToAzureFieldValue());
            Assert.Equal("251", new BigInteger(251).ToAzureFieldValue());
            Assert.Equal("7118507", new HexBigInteger(7118507).ToAzureFieldValue());
            Assert.Equal("0x26bc47888b7bfdf77db41ec0a2fb4db00af1c92a", "0x26bc47888b7bfdf77db41ec0a2fb4db00af1c92a".ToAzureFieldValue());
            Assert.Equal("hello", Encoding.UTF8.GetBytes("hello").ToAzureFieldValue());
        }

        [Fact]
        public void ToAzureField()
        {
            var eventSearchField = new SearchField("FieldA.Val")
            {
                DataType = typeof(string),
                IsFacetable = true,
                IsKey = true,
                IsFilterable = true,
                IsSortable = true,
                IsSearchable = true,
                IsSuggester = true
            };

            var azureField = eventSearchField.ToAzureField();
            Assert.Equal("fielda_val", azureField.Name);
            Assert.Equal(DataType.String, azureField.Type);
            Assert.True(azureField.IsFacetable);
            Assert.True(azureField.IsKey);
            Assert.True(azureField.IsFilterable);
            Assert.True(azureField.IsSortable);
            Assert.True(azureField.IsSearchable);
        }

        [Fact]
        public void ToAzureField_ConvertsToCollectionDataType()
        {
            var eventSearchField = new SearchField("FieldA.Val")
            {
                DataType = typeof(string),
                IsCollection = true
            };

            var azureField = eventSearchField.ToAzureField();
            Assert.Equal(DataType.Collection(DataType.String), azureField.Type);
        }

        [Fact]
        public void ToAzureFields()
        {
            var fields = new[]
            {
                new SearchField("FieldA.Prop1"),
                new SearchField("FieldB")
            };

            var azureSearchFields = fields.ToAzureFields();
            Assert.Equal(2, azureSearchFields.Length);
            Assert.Equal("fielda_prop1", azureSearchFields[0].Name);
            Assert.Equal("fieldb", azureSearchFields[1].Name);
        }

        [Fact]
        public void ToAzureSuggesters_FieldMustBeSuggesterAndSearchable()
        {
            var fields = new[]
            {
                //valid
                new SearchField("FieldA"){IsSuggester = true, IsSearchable = true},
                new SearchField("FieldB"){IsSuggester = true, IsSearchable = true},
                //invalid
                new SearchField("FieldC"){IsSuggester = true, IsSearchable = false},
                new SearchField("FieldD"){IsSuggester = false, IsSearchable = true},
                new SearchField("FieldE"){IsSuggester = false, IsSearchable = false}
            };

            var suggesters = fields.ToAzureSuggesters();
            Assert.Single(suggesters);
            Assert.Equal(2, suggesters[0].SourceFields.Count);
            Assert.Equal("fielda", suggesters[0].SourceFields[0]);
            Assert.Equal("fieldb", suggesters[0].SourceFields[1]);
        }

        [Fact]
        public void ToAzureIndex()
        {
            var eventSearchDefinition = new IndexDefinition("IndexA")
            {
                Fields = new []
                {
                    new SearchField("FieldA"),
                    new SearchField("FieldB")
                }
            };
            var azureIndex = eventSearchDefinition.ToAzureIndex();

            Assert.Equal("indexa", azureIndex.Name);
            Assert.Equal(eventSearchDefinition.Fields.Length, azureIndex.Fields.Count);
        }

        [Event("Transfer")]
        public class TransferEvent : IEventDTO
        {
            [Parameter("address", "_from", 1, true)]
            public string From {get; set;}

            [Parameter("address", "_to", 2, true)]
            public string To {get; set;}

            [Parameter("uint256", "_value", 3, true)]
            public BigInteger Value {get; set;}
        }

        [Fact]
        public void ToAzureDocument_CreatesDictionaryFromEventLogUsingIndexDefinition()
        {
            var indexDefinition = new EventIndexDefinition<TransferEvent>();

            var eventLog = new EventLog<TransferEvent>(
                new TransferEvent
                {
                    From = "0x9209b29f2094457d3dba62d1953efea58176ba27",
                    To = "0x1209b29f2094457d3dba62d1953efea58176ba28",
                    Value = new HexBigInteger(2000000)
                },
                new FilterLog
                {
                    Address = "0x26bc47888b7bfdf77db41ec0a2fb4db00af1c92a",
                    TransactionHash = "0xcb00b69d2594a3583309f332ada97d0df48bae00170e36a4f7bbdad7783fc7e5",
                    BlockNumber = new HexBigInteger(7118507),
                    BlockHash = "0x337cd6feedafac6abba40eff40fb1957e08985180f5a03016924ef72fc7b04b9",
                    LogIndex = new HexBigInteger(0),
                    Removed = false,
                    TransactionIndex = new HexBigInteger(0)
                });

            var doc = eventLog.ToAzureDocument(indexDefinition);

            Assert.IsType<Dictionary<string, object>>(doc);
            var dictionary = doc as Dictionary<string, object>;

            Assert.Equal(eventLog.Event.From, dictionary["from"]);
            Assert.Equal(eventLog.Event.To, dictionary["to"]);
            Assert.Equal(eventLog.Event.Value.ToString(), dictionary["value"]);

            Assert.Equal(eventLog.Log.Address, dictionary[PresetSearchFieldName.log_address.ToString()]);
            Assert.Equal(eventLog.Log.TransactionHash, dictionary[PresetSearchFieldName.log_transaction_hash.ToString()]);
            Assert.Equal(eventLog.Log.BlockNumber.Value.ToString(), dictionary[PresetSearchFieldName.log_block_number.ToString()]);
            Assert.Equal(eventLog.Log.BlockHash, dictionary[PresetSearchFieldName.log_block_hash.ToString()]);
            Assert.Equal(eventLog.Log.LogIndex.Value.ToString(), dictionary[PresetSearchFieldName.log_log_index.ToString()]);
            Assert.Equal(eventLog.Log.Removed, dictionary[PresetSearchFieldName.log_removed.ToString()]);
            Assert.Equal(eventLog.Log.TransactionIndex.Value.ToString(), dictionary[PresetSearchFieldName.log_transaction_index.ToString()]);
        }
    }
}
