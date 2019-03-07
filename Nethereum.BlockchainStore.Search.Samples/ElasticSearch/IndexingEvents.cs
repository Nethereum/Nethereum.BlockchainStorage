using Amazon;
using Amazon.Runtime;
using Elasticsearch.Net;
using Elasticsearch.Net.Aws;
using Nest;
using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.BlockchainStore.Search.ElasticSearch;
using Nethereum.Configuration;
using Nethereum.Contracts;
using System;
using System.Numerics;
using System.Threading.Tasks;
using Xunit;

namespace Nethereum.BlockchainStore.Search.Samples.ElasticSearch
{
    [Collection("Nethereum.BlockchainStore.Search.Samples.ElasticSearch")]
    public class IndexingEvents
    {
        /*
Solidity Contract Excerpt
* event Transfer(address indexed _from, address indexed _to, uint256 _value);
*/
        [Event("Transfer")]
        public class TransferEvent_ERC20
        {
            [Parameter("address", "_from", 1, true)]
            public string From {get; set;}

            [Parameter("address", "_to", 2, true)]
            public string To {get; set;}

            [Parameter("uint256", "_value", 3, false)]
            public BigInteger Value {get; set;}
        }

        private const string TransferIndexName = "transfer";
        private const string BlockchainUrl = "https://rinkeby.infura.io/v3/25e7b6dfc51040b3bfc0e47317d38f60";

        public static class ConfigurationKeyNames
        {
            public const string AWSElasticSearchUrl = "AWSElasticSearchUrl";
            public const string AWS_ACCESS_KEY_ID = "AWS_ACCESS_KEY_ID";
            public const string AWS_SECRET_ACCESS_KEY = "AWS_SECRET_ACCESS_KEY";
        }

        private string AwsSecretAccessKey;
        private string AwsAccessKeyId;
        private string AwsElasticSearchUrl;

        public IndexingEvents()
        {
            /*
             * Amazon EWS Search Secrets
             * AWSElasticSearchUrl
             * AWS_ACCESS_KEY_ID
             * AWS_SECRET_ACCESS_KEY
             */

            //user secrets are only for development
            //if not in development the key will be retrieved from environmental variables or command line args
            ConfigurationUtils.SetEnvironment("development");

            //use the command line to set your azure search api key
            //e.g. dotnet user-secrets set "AzureSearchApiKey" "<put key here>"
            var appConfig = ConfigurationUtils
                .Build(Array.Empty<string>(), userSecretsId: "Nethereum.BlockchainStore.Search.Samples");

            AwsAccessKeyId = appConfig[ConfigurationKeyNames.AWS_ACCESS_KEY_ID];
            AwsSecretAccessKey = appConfig[ConfigurationKeyNames.AWS_SECRET_ACCESS_KEY];
            AwsElasticSearchUrl = appConfig[ConfigurationKeyNames.AWSElasticSearchUrl];
        }

        /// <summary>
        /// Indexing events in the most simple way
        /// This example uses an Amazon hosted elastic search host
        /// However - it will work with any elastic search service
        /// Just configure the ElasticClient the way you need to
        /// </summary>
        [Fact]
        public async Task StartHere()
        {
            #region AWS Elastic Client Setup
            var httpConnection = new AwsHttpConnection(
                new BasicAWSCredentials(AwsAccessKeyId, AwsSecretAccessKey), RegionEndpoint.USEast2);

            var pool = new SingleNodeConnectionPool(new Uri(AwsElasticSearchUrl));
            var config = new ConnectionSettings(pool, httpConnection);
            var elasticClient = new ElasticClient(config);
            #endregion 

            using (var processor =
                new ElasticEventIndexingProcessor(
                    elasticClient, BlockchainUrl))
            {
                #region test preparation
                await processor.ClearProgress();
                await processor.SearchService.DeleteIndexAsync(TransferIndexName);
                #endregion

                var transferIndexProcessor = await processor.AddAsync<TransferEvent_ERC20>(TransferIndexName);

                var blocksProcessed = await processor.ProcessAsync(3146684, 3146694);

                Assert.Equal((ulong)11, blocksProcessed);
                Assert.Equal(1, processor.Indexers.Count);
                Assert.Equal(19, processor.Indexers[0].Indexed);

                await Task.Delay(TimeSpan.FromSeconds(5));
                Assert.Equal(19, await transferIndexProcessor.Indexer.DocumentCountAsync());

                #region test clean up 
                await processor.ClearProgress();
                await processor.SearchService.DeleteIndexAsync(TransferIndexName);
                #endregion
            }
        }

        /// <summary>
        /// An example of a custom DTO based on a transfer event to be stored in the azure index
        /// </summary>
        public class CustomTransferSearchDocumentDto: IHasId
        {
            public string From { get; set; }
            public string To { get; set; }
            public string Value { get; set; }
            public string BlockNumber { get; set; }
            public string TxHash { get; set; }
            public string LogAddress { get; set; }
            public int LogIndex { get; set; }

            public string DocumentKey { get; set; }

            /// <summary>
            /// a unique id for the elastic search doc
            /// </summary>
            public string GetId() => DocumentKey;
        }

        /// <summary>
        /// An example of a simple event to search document dto mapper
        /// </summary>
        public class CustomEventToSearchDocumentMapper :
            IEventToSearchDocumentMapper<TransferEvent_ERC20, CustomTransferSearchDocumentDto>
        {
            public CustomTransferSearchDocumentDto Map(EventLog<TransferEvent_ERC20> from)
            {
                return new CustomTransferSearchDocumentDto
                {
                    From = from.Event.From,
                    To = from.Event.To,
                    Value = from.Event.Value.ToString(),
                    BlockNumber = from.Log.BlockNumber.Value.ToString(),
                    TxHash = from.Log.TransactionHash,
                    LogAddress = from.Log.Address,
                    LogIndex = (int) from.Log.LogIndex.Value,
                    DocumentKey = $"{from.Log.TransactionHash}_{from.Log.LogIndex.Value}"
                };
            }
        }

        /// <summary>
        /// Dictating exactly what you want stored in the index - using a custom mapper to translate to a search document
        /// </summary>
        [Fact]
        public async Task StoringCustomSearchDocuments_UsingMapper()
        {
            #region AWS Elastic Client Setup
            var httpConnection = new AwsHttpConnection(
                new BasicAWSCredentials(AwsAccessKeyId, AwsSecretAccessKey), RegionEndpoint.USEast2);

            var pool = new SingleNodeConnectionPool(new Uri(AwsElasticSearchUrl));
            var config = new ConnectionSettings(pool, httpConnection);
            var elasticClient = new ElasticClient(config);
            #endregion 

            using (var processor =
                new ElasticEventIndexingProcessor(
                    elasticClient, BlockchainUrl))
            {
                #region test preparation
                await processor.ClearProgress();
                await processor.SearchService.DeleteIndexAsync(TransferIndexName);
                #endregion

                var mapper = new CustomEventToSearchDocumentMapper();

                //inject a mapping func to translate our event to a doc to store in the index
                await processor.AddAsync<TransferEvent_ERC20, CustomTransferSearchDocumentDto>(TransferIndexName, mapper);

                var blocksProcessed = await processor.ProcessAsync(3146684, 3146694);

                Assert.Equal((ulong)11, blocksProcessed);
                Assert.Equal(1, processor.Indexers.Count);
                Assert.Equal(19, processor.Indexers[0].Indexed);

                #region test clean up 
                await processor.ClearProgress();
                await processor.SearchService.DeleteIndexAsync(TransferIndexName);
                #endregion
            }
        }
    }
}
