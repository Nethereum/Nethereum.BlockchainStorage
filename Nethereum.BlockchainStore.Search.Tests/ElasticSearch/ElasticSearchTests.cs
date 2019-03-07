using Nest;
using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.BlockchainStore.Search.ElasticSearch;
using Nethereum.Contracts;
using Nethereum.Hex.HexTypes;
using Nethereum.RPC.Eth.DTOs;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Threading.Tasks;
using Xunit;

namespace Nethereum.BlockchainStore.Search.Tests.ElasticSearch
{
    public class ElasticSearchTests
    {
        [Event("Transfer")]
        public class TransferEvent : IEventDTO
        {
            [Parameter("address", "_from", 1, true)]
            public string From {get; set;}

            [Parameter("address", "_to", 2, true)]
            public string To {get; set;}

            [Parameter("uint256", "_value", 3, true)]
            public BigInteger Value {get; set;}

            [Parameter("tuple", "_detail", 4, false)]
            public TransferDetail Detail { get; set; }
        }

        public class TransferDetail
        {
            [Parameter("string", "_description", 1)]
            public string Description {get; set;}

            [Parameter("tuple[2]", "_tags", 2)]
            public List<Tag> Tags { get; set; }
        }

        public class Tag
        {
            [SearchField(IsSearchable = true, IsFacetable = true, IsFilterable = true)]
            [Parameter("string", "_description", 1, true)]
            public string Name {get; set;}

            [SearchField(IsSearchable = true, IsFacetable = true, IsFilterable = true)]
            [Parameter("string", "_value", 2, true)]
            public string Value {get; set;}
        }

        [Fact]
        public async Task IndexerTest_WithGenericSearchDocDto()
        {
            var elasticClient = new ElasticClient(new Uri("http://localhost:9200/"));
            var eventIndexDefinition = new EventIndexDefinition<TransferEvent>();
            var indexName = eventIndexDefinition.ElasticIndexName();

            try
            {
                await DeleteIndex(elasticClient, indexName);

                var mappings = eventIndexDefinition.CreateElasticMappings();

                var createIndexResponse =
                    await elasticClient.CreateIndexAsync(new CreateIndexRequest(indexName)
                    {
                        Mappings = mappings
                    });

                Assert.True(createIndexResponse.IsValid);

                var indexer =
                    new ElasticEventIndexer<TransferEvent>(elasticClient, eventIndexDefinition);

                var log = CreateTransferEvent();
                await indexer.IndexAsync(log);
                Assert.Equal(1, indexer.Indexed);
                await Task.Delay(TimeSpan.FromSeconds(5));
                Assert.Equal(1, await indexer.DocumentCountAsync());
            }
            finally
            {
                await DeleteIndex(elasticClient, indexName);
            }
        }

        /// <summary>
        /// a simple search document DTO example for a transfer
        /// GetId is used when indexing the doc as the unique id
        /// </summary>
        public class TransferSearchDocDto : IHasId
        {
            public string Id { get; set; }

            public string GetId() => Id;

            public string From { get; set; }
            public string To { get;set; }
            public string Value { get; set; }   
        }

        /// <summary>
        /// an example of a mapper from block chain DTOs to a specific DTO to store in the search index
        /// </summary>
        public class EventToTransferSearchDocMapper : IEventToSearchDocumentMapper<TransferEvent, TransferSearchDocDto>
        {
            public TransferSearchDocDto Map(EventLog<TransferEvent> from)
            {
                return new TransferSearchDocDto
                {
                    Id = $"{from.Log.TransactionHash}_{from.Log.LogIndex.Value}",
                    From = from.Event.From,
                    To = from.Event.To,
                    Value = from.Event.Value.ToString()
                };
            }
        }

        [Fact]
        public async Task IndexerTest_WithSpecificSearchDocDto()
        {
            var elasticClient = new ElasticClient(new Uri("http://localhost:9200/"));
            var indexName = "transfer";

            try
            {
                await DeleteIndex(elasticClient, indexName);

                //deliberately not dictating index properties/fields
                //relying on elastic dynamically creating meta data during indexing
                var createIndexResponse =
                    await elasticClient.CreateIndexAsync(new CreateIndexRequest(indexName));

                Assert.True(createIndexResponse.IsValid);

                var mapper = new EventToTransferSearchDocMapper();

                var indexer =
                    new ElasticEventIndexer<TransferEvent, TransferSearchDocDto>(elasticClient, indexName, mapper);

                var log = CreateTransferEvent();
                await indexer.IndexAsync(log);
                Assert.Equal(1, indexer.Indexed);
                await Task.Delay(TimeSpan.FromSeconds(5));
                Assert.Equal(1, await indexer.DocumentCountAsync());
            }
            finally
            {
                await DeleteIndex(elasticClient, indexName);
            }
        }

        private static async Task DeleteIndex(ElasticClient elasticClient, string indexName)
        {
            var existsResponse = await elasticClient.IndexExistsAsync(new IndexExistsRequest(indexName));
            if (existsResponse.Exists)
            {
                await elasticClient.DeleteIndexAsync(indexName);
            }
        }

        private EventLog<TransferEvent> CreateTransferEvent()
        {
            var transferEventLog = new EventLog<TransferEvent>(
                new TransferEvent
                {
                    From = "0x9209b29f2094457d3dba62d1953efea58176ba27",
                    To = "0x1209b29f2094457d3dba62d1953efea58176ba28",
                    Value = new HexBigInteger("2000000"),
                    Detail = new TransferDetail
                    {
                        Description = "A generic transfer",
                        Tags = new List<Tag>
                        {
                            new Tag{Name = "Status", Value = "Good"},
                            new Tag{Name = "Year", Value = "2019"}
                        }
                    }
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

            return transferEventLog;
        }
    }
}
