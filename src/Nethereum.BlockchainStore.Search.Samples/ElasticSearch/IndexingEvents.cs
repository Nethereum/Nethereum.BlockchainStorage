using Amazon;
using Amazon.Runtime;
using Elasticsearch.Net;
using Elasticsearch.Net.Aws;
using Nest;
using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.BlockchainStore.Search.ElasticSearch;
using Nethereum.Contracts;
using Nethereum.Microsoft.Configuration.Utils;
using System;
using System.Numerics;
using System.Threading;
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
        public class TransferEvent_ERC20: IEventDTO
        {
            [Parameter("address", "_from", 1, true)]
            public string From { get; set; }

            [Parameter("address", "_to", 2, true)]
            public string To { get; set; }

            [Parameter("uint256", "_value", 3, false)]
            public BigInteger Value { get; set; }
        }

        private const string BlockchainUrl = TestConfiguration.BlockchainUrls.Infura.Rinkeby;

        public static class ConfigurationKeyNames
        {
            public const string AWSElasticSearchUrl = "AWSElasticSearchUrl";
            public const string AWS_ACCESS_KEY_ID = "AWS_ACCESS_KEY_ID";
            public const string AWS_SECRET_ACCESS_KEY = "AWS_SECRET_ACCESS_KEY";
            public const string Disable_Elastic_Search_Samples = "Disable_Elastic_Search_Samples";
        }

        private readonly string _awsSecretAccessKey;
        private readonly string _awsAccessKeyId;
        private readonly string _awsElasticSearchUrl;
        private readonly bool _disableElasticSearch;

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
            ConfigurationUtils.SetEnvironmentAsDevelopment();

            //use the command line to set your azure search api key
            //e.g. dotnet user-secrets set "AWS_ACCESS_KEY_ID" "<put key here>"
            var appConfig = ConfigurationUtils
                .Build(Array.Empty<string>(), userSecretsId: "Nethereum.BlockchainStore.Search.Samples");

            _awsAccessKeyId = appConfig[ConfigurationKeyNames.AWS_ACCESS_KEY_ID];
            _awsSecretAccessKey = appConfig[ConfigurationKeyNames.AWS_SECRET_ACCESS_KEY];
            _awsElasticSearchUrl = appConfig[ConfigurationKeyNames.AWSElasticSearchUrl];

            // disable from CI
            var disableElasticSearchValue = appConfig[ConfigurationKeyNames.Disable_Elastic_Search_Samples];
            if(bool.TryParse(disableElasticSearchValue, out bool disable))
            {
                _disableElasticSearch = disable;
            }
        }

        [Fact]
        public async Task OneEvent()
        {
            if(_disableElasticSearch) return;

            const string INDEX_NAME = "transfer-logs";

            var elasticClient = CreateLocalElasticClient();

            //surround with "using" so that anything in a buffer is sent on dispose
            using (var elasticSearchService = new ElasticSearchService(elasticClient))
            {
                try
                {
                    //setup
                    await elasticSearchService.CreateIfNotExists(INDEX_NAME);

                    var indexer = elasticSearchService.CreateIndexerForEventLog<TransferEvent_ERC20>(INDEX_NAME, documentsPerBatch: 1);

                    var web3 = new Web3.Web3(BlockchainUrl);
                    var blockchainProcessor = web3.Processing.Logs.CreateProcessor<TransferEvent_ERC20>((transfer) => indexer.IndexAsync(transfer));
                    var cancellationTokenSource = new CancellationTokenSource();

                    //execute
                    await blockchainProcessor.ExecuteAsync(3146694, cancellationTokenSource.Token, 3146684);

                    //assert
                    await Task.Delay(1000); // allow time to index
                    Assert.Equal(19, await elasticSearchService.CountDocumentsAsync(INDEX_NAME));

                    //http://localhost:9200/transfer-logs/_search?q=from:0x1b31d19b6a9a942bbf3c197ca1e5efede3ff8ff2

                }
                finally
                {
                    await elasticSearchService.DeleteIndexAsync(INDEX_NAME);
                }
            }
        }

        [Fact]
        public async Task OneEventWithMapping()
        {
            if (_disableElasticSearch) return;

            const string INDEX_NAME = "mapped-transfer-logs";

            var elasticClient = CreateLocalElasticClient();

            //surround with "using" so that anything in a buffer is sent on dispose
            using (var elasticSearchService = new ElasticSearchService(elasticClient))
            {
                try
                {
                    //setup
                    await elasticSearchService.CreateIfNotExists(INDEX_NAME);

                    var indexer = elasticSearchService.CreateIndexerForEventLog<TransferEvent_ERC20, CustomTransferSearchDocumentDto>(
                        INDEX_NAME, 
                        (from) => new CustomTransferSearchDocumentDto
                        {
                            From = from.Event.From,
                            To = from.Event.To,
                            Value = from.Event.Value.ToString(),
                            BlockNumber = from.Log.BlockNumber.Value.ToString(),
                            TxHash = from.Log.TransactionHash,
                            LogAddress = from.Log.Address,
                            LogIndex = (int)from.Log.LogIndex.Value,
                            DocumentKey = $"{from.Log.TransactionHash}_{from.Log.LogIndex.Value}"
                        }, 
                        documentsPerBatch: 1);

                    var web3 = new Web3.Web3(BlockchainUrl);
                    var blockchainProcessor = web3.Processing.Logs.CreateProcessor<TransferEvent_ERC20>((transfer) => indexer.IndexAsync(transfer));
                    var cancellationTokenSource = new CancellationTokenSource();

                    //execute
                    await blockchainProcessor.ExecuteAsync(3146694, cancellationTokenSource.Token, 3146684);

                    //assert
                    await Task.Delay(1000); // allow time to index
                    Assert.Equal(19, await elasticSearchService.CountDocumentsAsync(INDEX_NAME));

                    //http://localhost:9200/transfer-logs/_search?q=from:0x1b31d19b6a9a942bbf3c197ca1e5efede3ff8ff2

                }
                finally
                {
                    await elasticSearchService.DeleteIndexAsync(INDEX_NAME);
                }
            }
        }

        private ElasticClient CreateAwsHostedElasticClient()
        {
            var httpConnection = new AwsHttpConnection(
                new BasicAWSCredentials(_awsAccessKeyId, _awsSecretAccessKey), RegionEndpoint.USEast2);
            var pool = new SingleNodeConnectionPool(new Uri(_awsElasticSearchUrl));
            var config = new ConnectionSettings(pool, httpConnection);
            var elasticClient = new ElasticClient(config);
            return elasticClient;
        }

        private ElasticClient CreateLocalElasticClient()
        {
            var elasticClient = new ElasticClient(new Uri("http://localhost:9200"));
            return elasticClient;
        }

        /// <summary>
        /// An example of a custom DTO based on a transfer event to be stored in the azure index
        /// </summary>
        public class CustomTransferSearchDocumentDto : IHasId
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

    }
}
