using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.BlockchainProcessing.BlockchainProxy;
using Nethereum.BlockchainProcessing.Processing;
using Nethereum.BlockchainProcessing.Processing.Logs;
using Nethereum.BlockchainStore.Search.Azure;
using Nethereum.Configuration;
using Nethereum.Contracts;
using System;
using System.IO;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Nethereum.BlockchainStore.Search.Samples
{
    public class IndexingTransferEvents
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

            public TransferMetadata Metadata { get; set; } = new TransferMetadata();
        }

        /*
Solidity Contract Excerpt
* event Transfer(address indexed _from, address indexed _to, uint256 indexed _value);
*/
        [Event("Transfer")]
        public class TransferEvent_Custom
        {
            [Parameter("address", "_from", 1, true)]
            public string From {get; set;}

            [Parameter("address", "_to", 2, true)]
            public string To {get; set;}

            [Parameter("uint256", "_value", 3, true)]
            public BigInteger Value {get; set;}

            public TransferMetadata Metadata { get; set; } = new TransferMetadata();
        }

        public class TransferMetadata
        {
            public static string CurrentChainUrl { get;set; }

            public TransferMetadata()
            {
                IndexedOn = DateTimeOffset.Now.ToUnixTimeSeconds().ToString();
                ChainUrl = CurrentChainUrl;
                IndexingMachineName = Environment.MachineName;
            }

            [SearchField]
            public string IndexedOn { get; set; }

            [SearchField(IsSearchable = true)]
            public string ChainUrl { get;set; }

            [SearchField(IsSearchable = true)]
            public string IndexingMachineName { get;set; }
        }

        public const string ApiKeyName = "AzureSearchApiKey";
        public const string AzureSearchServiceName = "blockchainsearch";
        public const string AzureTransferIndexName = "transfer";

        private readonly string _azureSearchApiKey;

        public IndexingTransferEvents()
        {
            //user secrets are only for development
            //if not in development the key will be retrieved from environmental variables or command line args
            ConfigurationUtils.SetEnvironment("development");

            //use the command line to set your azure search api key
            //e.g. dotnet user-secrets set "AzureSearchApiKey" "<put key here>"
            var appConfig = ConfigurationUtils
                .Build(Array.Empty<string>(), userSecretsId: "Nethereum.BlockchainStore.Search.Samples");

            _azureSearchApiKey = appConfig[ApiKeyName];
        }


        [Fact]
        public async Task ERC20()
        {
            const string chainUrl = "https://rinkeby.infura.io/v3/25e7b6dfc51040b3bfc0e47317d38f60";
            TransferMetadata.CurrentChainUrl = chainUrl;

            var blockchainProxyService =
                new BlockchainProxyService(chainUrl);

            using (var azureSearchService = new AzureEventSearchService(AzureSearchServiceName, _azureSearchApiKey))
            {
                await azureSearchService.DeleteIndexAsync(AzureTransferIndexName);

                using (var transferIndexer = await azureSearchService.GetOrCreateEventIndex<TransferEvent_ERC20>(AzureTransferIndexName))
                {
                    using (var transferProcessor =
                        new EventIndexProcessor<TransferEvent_ERC20>(transferIndexer))
                    {

                        var logProcessor = new BlockchainLogProcessor(
                            blockchainProxyService,
                            new ILogProcessor[] {transferProcessor});

                        var progressRepository =
                            new JsonBlockProgressRepository(CreateJsonFileToHoldProgress());

                        var progressService = new StaticBlockRangeProgressService(
                            3146684, 3146694, progressRepository);

                        var batchProcessorService = new BlockchainBatchProcessorService(
                            logProcessor, progressService, maxNumberOfBlocksPerBatch: 2);

                        BlockRange? lastBlockRangeProcessed;
                        do
                        {
                            lastBlockRangeProcessed = await batchProcessorService.ProcessLatestBlocksAsync();
                        } while (lastBlockRangeProcessed != null);

                        Assert.Equal(19, transferIndexer.Indexed);
                    }
                }

                await azureSearchService.DeleteIndexAsync(AzureTransferIndexName);
            }
        }
    
        [Fact]
        public async Task ERC20_And_Derivatives()
        {
            var blockchainProxyService = new BlockchainProxyService("https://rinkeby.infura.io/v3/25e7b6dfc51040b3bfc0e47317d38f60");

            //using disposable context to restrict the amount of nesting in the code example
            using (var d = new DisposableContext())
            {
                var searchService = d.Add(() => new AzureEventSearchService(AzureSearchServiceName, _azureSearchApiKey));
                
                await searchService.DeleteIndexAsync(AzureTransferIndexName);

                //we'll add the custom event first
                //this will create the index in azure
                //this has an indexable "value" element and we want that to be searchable in azure
                var customIndexer = await d.Add(() => searchService.GetOrCreateEventIndex<TransferEvent_Custom>(AzureTransferIndexName));
                var customProcessor = d.Add(() => new EventIndexProcessor<TransferEvent_Custom>(customIndexer));

                //add the erc 20 event -
                //this will use the same azure index as the custom event - it won't be re-created or altered
                //the "value" element will still be indexed in azure despite not being indexed in ethereum
                var erc20Indexer = await d.Add(() => searchService.GetOrCreateEventIndex<TransferEvent_ERC20>(AzureTransferIndexName));
                var erc20Processor = d.Add(() => new EventIndexProcessor<TransferEvent_ERC20>(erc20Indexer));

                var logProcessor = new BlockchainLogProcessor(
                    blockchainProxyService,
                    new ILogProcessor[] {erc20Processor, customProcessor});

                var progressRepository =
                    new JsonBlockProgressRepository(CreateJsonFileToHoldProgress());

                var progressService = new StaticBlockRangeProgressService(
                    3146684, 3146694, progressRepository);

                var batchProcessorService = new BlockchainBatchProcessorService(
                    logProcessor, progressService, maxNumberOfBlocksPerBatch: 2);

                BlockRange? lastBlockRangeProcessed;
                do
                {
                    lastBlockRangeProcessed = await batchProcessorService.ProcessLatestBlocksAsync();
                } while (lastBlockRangeProcessed != null);

                Assert.Equal(19, erc20Indexer.Indexed);
                Assert.Equal(6, customIndexer.Indexed);

                await Task.Delay(5000); // leave time for index

                //the indexers wrap the same underlying azure index
                //this azure index should have documents for both transfer event types
                Assert.Equal(25, await erc20Indexer.DocumentCountAsync());
                Assert.Equal(25, await customIndexer.DocumentCountAsync());

                await searchService.DeleteIndexAsync(AzureTransferIndexName);
            }
        }

        private static string CreateJsonFileToHoldProgress()
        {
            var progressFileNameAndPath = Path.Combine(Path.GetTempPath(), "BlockProcess.json");
            if (File.Exists(progressFileNameAndPath)) File.Delete(progressFileNameAndPath);
            return progressFileNameAndPath;
        }

        [Fact(Skip = "POC")]
        public async Task RunAsAServiceFor10Minutes()
        {
            using (var service = new POC.TransferIndexingService(
                "https://rinkeby.infura.io/v3/25e7b6dfc51040b3bfc0e47317d38f60",
                AzureSearchServiceName,
                _azureSearchApiKey,
                AzureTransferIndexName))
            {
                var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromMinutes(10));
                await service.RunContinually(fromBlockNumber: 3770762, cancellationToken: cancellationTokenSource.Token);
            }

            //sample azure query
            //search=3770910&searchFields=log_block_number&$count=true&$select=log_block_number,log_transaction_index,from,to,value
        } 
    }
}

