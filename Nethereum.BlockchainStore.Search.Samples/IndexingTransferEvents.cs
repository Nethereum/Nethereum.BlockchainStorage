using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.BlockchainProcessing.BlockchainProxy;
using Nethereum.BlockchainProcessing.Processing;
using Nethereum.BlockchainProcessing.Processing.Logs;
using Nethereum.BlockchainStore.Search.Azure;
using Nethereum.Configuration;
using Nethereum.Contracts;
using System;
using System.Collections.Generic;
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
        }

        [Event("Transfer")]
        public class TransferEvent_Extended
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
        public class TransferEvent_With3Indexes
        {
            [Parameter("address", "_from", 1, true)]
            public string From {get; set;}

            [Parameter("address", "_to", 2, true)]
            public string To {get; set;}

            //note the 3rd parameter is indexed (unlike ERC20)
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
        private const string BlockchainUrl = "https://rinkeby.infura.io/v3/25e7b6dfc51040b3bfc0e47317d38f60";
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
        public async Task StartHere()
        {
            using (var processor =
                new AzureEventIndexingProcessor(AzureSearchServiceName, _azureSearchApiKey, BlockchainUrl))
            {
                #region test preparation
                await processor.ClearProgress();
                await processor.SearchService.DeleteIndexAsync(AzureTransferIndexName);
                #endregion

                await processor.AddEventAsync<TransferEvent_ERC20>(AzureTransferIndexName);

                var blocksProcessed = await processor.ProcessAsync(3146684, 3146694);

                Assert.Equal((ulong)11, blocksProcessed);
                Assert.Equal(1, processor.Indexers.Count);
                Assert.Equal(6, processor.Indexers[0].Indexed);

                #region test clean up 
                await processor.ClearProgress();
                await processor.SearchService.DeleteIndexAsync(AzureTransferIndexName);
                #endregion
            }
        }

        [Fact]
        public async Task RunContinually()
        {
            var web3 = new Web3.Web3(BlockchainUrl);
            var currentBlockNumber = (ulong)(await web3.Eth.Blocks.GetBlockNumber.SendRequestAsync()).Value;
            var startingBlock = currentBlockNumber - 10;
            var maxBlock = currentBlockNumber + 1;

            const ulong expectedBlocks = 12;  // current block + 10 + 1

            using (var processor =
                new AzureEventIndexingProcessor(
                    AzureSearchServiceName, _azureSearchApiKey, BlockchainUrl, maxBlocksPerBatch: 1, minBlockConfirmations: 0))
            {
                #region test preparation
                await processor.ClearProgress();
                await processor.SearchService.DeleteIndexAsync(AzureTransferIndexName);
                #endregion

                await processor.AddEventAsync<TransferEvent_ERC20>(AzureTransferIndexName);

                var cancellationToken = new CancellationTokenSource();
                var escapeHatch = new Action<uint, BlockRange>((rangesProcessed, lastRange) =>
                {
                    if (lastRange.To >= maxBlock) // escape hatch!
                    {
                        cancellationToken.Cancel();
                    }
                });

                var blocksProcessed = await processor.ProcessAsync(startingBlock, 
                    ctx: cancellationToken, rangeProcessedCallback: escapeHatch);

                Assert.Equal(expectedBlocks, blocksProcessed);
 
                #region test clean up 
                await processor.ClearProgress();
                await processor.SearchService.DeleteIndexAsync(AzureTransferIndexName);
                #endregion
            }
        }

        [Fact]
        public async Task WithAFilter()
        {
            var filter = new NewFilterInputBuilder<TransferEvent_ERC20>()
                .AddTopic(tfr => tfr.To, "0xdfa70b70b41d77a7cdd8b878f57521d47c064d8c")
                .Build(contractAddress: "0x3678FbEFC663FC28336b93A1FA397B67ae42114d",
                    blockRange: new BlockRange(3860820, 3860820));
            
            using (var processor =
                new AzureEventIndexingProcessor(
                    AzureSearchServiceName, 
                    _azureSearchApiKey, 
                    BlockchainUrl, 
                    filters: new[] {filter}))
            {
                #region test preparation
                await processor.ClearProgress();
                await processor.SearchService.DeleteIndexAsync(AzureTransferIndexName);
                #endregion

                await processor.AddEventAsync<TransferEvent_ERC20>(AzureTransferIndexName);

                var blocksProcessed = await processor.ProcessAsync(3860820, 3860820);

                Assert.Equal((ulong)1, blocksProcessed);
                Assert.Equal(1, processor.Indexers[0].Indexed);

                #region test clean up 
                await processor.ClearProgress();
                await processor.SearchService.DeleteIndexAsync(AzureTransferIndexName);
                #endregion
            }
        }

        [Fact]
        public async Task CustomComposition()
        {
            TransferMetadata.CurrentChainUrl = BlockchainUrl;

            var blockchainProxyService =
                new BlockchainProxyService(BlockchainUrl);

            using (var azureSearchService = new AzureSearchService(AzureSearchServiceName, _azureSearchApiKey))
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
        public async Task IndexingTransferEventsWithDifferingSignatures()
        {
            using (var processor =
                new AzureEventIndexingProcessor(AzureSearchServiceName, _azureSearchApiKey, BlockchainUrl))
            {
                #region test preparation
                await processor.ClearProgress();
                await processor.SearchService.DeleteIndexAsync(AzureTransferIndexName);
                #endregion

                await processor.AddEventAsync<TransferEvent_With3Indexes>(AzureTransferIndexName);
                await processor.AddEventAsync<TransferEvent_ERC20>(AzureTransferIndexName);

                var blocksProcessed = await processor.ProcessAsync(3146684, 3146694);

                Assert.Equal((ulong)11, blocksProcessed);
                Assert.Equal(2, processor.Indexers.Count);
                Assert.Equal(6, processor.Indexers[0].Indexed);
                Assert.Equal(19, processor.Indexers[1].Indexed);

                await Task.Delay(5000); // leave time for index

                var customIndexer = processor.Indexers[0] as IAzureSearchIndex;
                var erc20Indexer = processor.Indexers[1] as IAzureSearchIndex;
               
                //the indexers wrap the same underlying azure index
                //this azure index should have documents for both transfer event types
                Assert.Equal(25, await erc20Indexer.DocumentCountAsync());
                Assert.Equal(25, await customIndexer.DocumentCountAsync());

                #region test clean up 
                await processor.ClearProgress();
                await processor.SearchService.DeleteIndexAsync(AzureTransferIndexName);
                #endregion
            }
        }

        [Fact]
        public async Task IndexingAdditionalInformationPerEvent()
        {
            using (var processor =
                new AzureEventIndexingProcessor(AzureSearchServiceName, _azureSearchApiKey, BlockchainUrl))
            {
                #region test preparation
                await processor.ClearProgress();
                await processor.SearchService.DeleteIndexAsync(AzureTransferIndexName);
                #endregion

                await processor.AddEventAsync<TransferEvent_Extended>(AzureTransferIndexName);

                await processor.ProcessAsync(3146684, 3146694);

                await Task.Delay(5000); // leave time for index

                var customIndexer = processor.Indexers[0] as IAzureSearchIndex;

                var query = "0x90df9bcd9608696df90c0baf5faefd2399bba0d2";
                var searchResult = await customIndexer.SearchAsync(query, new List<string>());

                Assert.Equal(1, searchResult.Count);

                //all should have our custom metadata which is not provided by the blockchain
                foreach (var result in searchResult.Results)
                {
                    Assert.Equal(Environment.MachineName, result.Document["metadata_indexingmachinename"]);
                }

                #region test clean up 
                await processor.ClearProgress();
                await processor.SearchService.DeleteIndexAsync(AzureTransferIndexName);
                #endregion
            }
        }
    
        private static string CreateJsonFileToHoldProgress()
        {
            var progressFileNameAndPath = Path.Combine(Path.GetTempPath(), "BlockProcess.json");
            if (File.Exists(progressFileNameAndPath)) File.Delete(progressFileNameAndPath);
            return progressFileNameAndPath;
        }

    }
}

