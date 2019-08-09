using Nethereum.BlockchainStore.Search.Azure;
using Nethereum.Contracts;
using System;
using System.Linq;
using Xunit;

namespace Nethereum.BlockchainStore.Search.Tests.Azure
{
    public class FilterLogIndexUtilTests
    {
        [Fact]
        public void CreatesAnAzureIndexDefinitionContainingAllFilterLogFields()
        {
            var index = FilterLogIndexUtil.Create("my-index");

            foreach(var fieldName in Enum.GetNames(typeof(PresetSearchFieldName)).Where(n => n.StartsWith("log_")))
            { 
                Assert.Contains(index.Fields, f => f.Name == fieldName);
            }
        }

        [Fact]
        public void CreatesExpectedKeyField()
        {
            var index = FilterLogIndexUtil.Create("my-index");
            var keyFields = index.Fields.Where(f => f.IsKey).ToArray();
            Assert.Single(keyFields);
            Assert.Contains(keyFields, f => f.Name == PresetSearchFieldName.log_key.ToString());
        }

        [Fact]
        public void FacetableFields()
        {
            var index = FilterLogIndexUtil.Create("my-index");
            var facetableFields = index.Fields.Where(f => f.IsFacetable).ToArray();

            var expectedFields = new []
            {
                PresetSearchFieldName.log_address, 
                PresetSearchFieldName.log_block_number, 
                PresetSearchFieldName.log_transaction_hash, 
                PresetSearchFieldName.log_topics
            };

            CheckFields(facetableFields, expectedFields);
        }

        [Fact]
        public void SearchableFields()
        {
            var index = FilterLogIndexUtil.Create("my-index");
            var searchableFields = index.Fields.Where(f => f.IsSearchable).ToArray();

            var expectedFields = new[] 
            { 
                PresetSearchFieldName.log_address, 
                PresetSearchFieldName.log_block_number, 
                PresetSearchFieldName.log_transaction_hash, 
                PresetSearchFieldName.log_topics 
            };

            CheckFields(searchableFields, expectedFields);
        }

        [Fact]
        public void FilterableFields()
        {
            var index = FilterLogIndexUtil.Create("my-index");
            var filterableFields = index.Fields.Where(f => f.IsFilterable).ToArray();

            var expectedFields = new[]
            {
                PresetSearchFieldName.log_removed,
                PresetSearchFieldName.log_type,
                PresetSearchFieldName.log_log_index,
                PresetSearchFieldName.log_transaction_hash,
                PresetSearchFieldName.log_transaction_index,
                PresetSearchFieldName.log_block_number,
                PresetSearchFieldName.log_address,
                PresetSearchFieldName.log_topics
             };

            CheckFields(filterableFields, expectedFields);
        }

        [Fact]
        public void RetrievableFields()
        {
            var index = FilterLogIndexUtil.Create("my-index");

            var allFieldCount = Enum.GetNames(typeof(PresetSearchFieldName)).Where(n => n.StartsWith("log_")).Count();
            Assert.Equal(allFieldCount, index.Fields.Count(f => f.IsRetrievable));
            
        }

        [Fact]
        public void MapsFilterLogIntoDictionary()
        {
            var sourceLog = TestData.Contracts.StandardContract.SampleTransferLog();
            var mapped = FilterLogIndexUtil.Map(sourceLog);
            
            Assert.Equal(sourceLog.Key(), mapped[PresetSearchFieldName.log_key.ToString()]);
            Assert.Equal(sourceLog.Removed, mapped[PresetSearchFieldName.log_removed.ToString()]);
            Assert.Equal(sourceLog.Type, mapped[PresetSearchFieldName.log_type.ToString()]);
            Assert.Equal(sourceLog.LogIndex.Value.ToString(), mapped[PresetSearchFieldName.log_log_index.ToString()]);
            Assert.Equal(sourceLog.TransactionHash, mapped[PresetSearchFieldName.log_transaction_hash.ToString()]);
            Assert.Equal(sourceLog.TransactionIndex.Value.ToString(), mapped[PresetSearchFieldName.log_transaction_index.ToString()]);
            Assert.Equal(sourceLog.BlockHash, mapped[PresetSearchFieldName.log_block_hash.ToString()]);
            Assert.Equal(sourceLog.BlockNumber.Value.ToString(), mapped[PresetSearchFieldName.log_block_number.ToString()]);
            Assert.Equal(sourceLog.Address, mapped[PresetSearchFieldName.log_address.ToString()]);
            Assert.Equal(sourceLog.Data, mapped[PresetSearchFieldName.log_data.ToString()]);

            var actualTopics = mapped[PresetSearchFieldName.log_topics.ToString()] as object[];

            for (var i = 0; i < sourceLog.Topics.Length; i++) 
            { 
                Assert.Equal(sourceLog.Topics[i], actualTopics[i]);
            }

        }

        private static void CheckFields(global::Microsoft.Azure.Search.Models.Field[] actualFields, PresetSearchFieldName[] expectedFields)
        {
            Assert.Equal(expectedFields.Length, actualFields.Length);

            foreach (var expectedField in expectedFields)
            {
                Assert.Contains(actualFields, f => f.Name == expectedField.ToString());
            }
        }
    }
}
