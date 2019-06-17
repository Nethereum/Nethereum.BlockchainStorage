using Nethereum.ABI.FunctionEncoding.Attributes;
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
using Microsoft.Azure.Search.Models;
using Xunit;
using Nethereum.LogProcessing;

namespace Nethereum.BlockchainStore.Search.Samples.Azure
{
    [Collection("Nethereum.BlockchainStore.Search.Samples.Azure")]
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

            [SearchField(IsSearchable = true, IsFacetable = true)]
            public string IndexingMachineName { get;set; }
        }

        public const string ApiKeyName = "AzureSearchApiKey";
        public const string AzureSearchServiceName = "blockchainsearch";
        public const string AzureTransferIndexName = "transfer";
        private const string BlockchainUrl = TestConfiguration.BlockchainUrls.Infura.Rinkeby;
        private readonly string _azureSearchApiKey;

        public IndexingTransferEvents()
        {
            //user secrets are only for development
            //if not in development the key will be retrieved from environmental variables or command line args
            ConfigurationUtils.SetEnvironmentAsDevelopment();

            //use the command line to set your azure search api key
            //e.g. dotnet user-secrets set "AzureSearchApiKey" "<put key here>"
            var appConfig = ConfigurationUtils
                .Build(Array.Empty<string>(), userSecretsId: "Nethereum.BlockchainStore.Search.Samples");

            _azureSearchApiKey = appConfig[ApiKeyName];
        }

        /// <summary>
        /// Indexing events in the most simple way
        /// </summary>
        [Fact]
        public async Task StartHere()
        {
            using (var processor =
                new AzureEventIndexingProcessor(AzureSearchServiceName, _azureSearchApiKey, BlockchainUrl))
            {
                await ClearDown(processor);
                try
                {
                    var transferEventProcessor = await processor.AddAsync<TransferEvent_ERC20>(AzureTransferIndexName);

                    var blocksProcessed = await processor.ProcessAsync(3146684, 3146694);

                    Assert.Equal((ulong)11, blocksProcessed);
                    Assert.Equal(19, transferEventProcessor.Indexer.Indexed);
                }
                finally
                {
                    await ClearDown(processor);
                }
            }
        }

        /// <summary>
        /// Dictating exactly what you want stored in the index - using a mapping func to translate from the event
        /// </summary>
        [Fact]
        public async Task StoringCustomSearchDocuments_UsingMapping()
        {
            using (var processor =
                new AzureEventIndexingProcessor(AzureSearchServiceName, _azureSearchApiKey, BlockchainUrl))
            {
                await ClearDown(processor);

                //create an azure index definition based on a custom Dto
                //the processor will create the index if it does not already exist
                Index index = CreateAzureIndexDefinition();

                try
                {
                    //inject a mapping func to translate our event to a doc to store in the index
                    await processor.AddAsync<TransferEvent_ERC20, CustomTransferSearchDocumentDto>(index, (e) =>
                        new CustomTransferSearchDocumentDto
                        {
                            From = e.Event.From,
                            To = e.Event.To,
                            Value = e.Event.Value.ToString(),
                            BlockNumber = e.Log.BlockNumber.Value.ToString(),
                            TxHash = e.Log.TransactionHash,
                            LogAddress = e.Log.Address,
                            LogIndex = (int)e.Log.LogIndex.Value,
                            DocumentKey = $"{e.Log.TransactionHash}_{e.Log.LogIndex.Value}"
                        });

                    var blocksProcessed = await processor.ProcessAsync(3146684, 3146694);

                    Assert.Equal((ulong)11, blocksProcessed);
                    Assert.Equal(1, processor.Indexers.Count);
                    Assert.Equal(19, processor.Indexers[0].Indexed);
                }
                finally
                {
                    await ClearDown(processor);
                }
            }
        }

        /// <summary>
        /// Dictating exactly what you want stored in the index - using a custom mapper to translate from the event
        /// </summary>
        [Fact]
        public async Task StoringCustomSearchDocuments_UsingMapper()
        {
            using (var processor =
                new AzureEventIndexingProcessor(AzureSearchServiceName, _azureSearchApiKey, BlockchainUrl))
            {
                await ClearDown(processor);

                //create an azure index definition based on a custom Dto
                //the processor will create the index if it does not already exist
                var index = CreateAzureIndexDefinition();

                var mapper = new CustomEventToSearchDocumentMapper();

                try
                {
                    //inject a mapping func to translate our event to a doc to store in the index
                    await processor.AddAsync<TransferEvent_ERC20, CustomTransferSearchDocumentDto>(index, mapper);

                    var blocksProcessed = await processor.ProcessAsync(3146684, 3146694);

                    Assert.Equal((ulong) 11, blocksProcessed);
                    Assert.Equal(1, processor.Indexers.Count);
                    Assert.Equal(19, processor.Indexers[0].Indexed);
                }
                finally
                {
                    await ClearDown(processor);
                }
            }
        }

        /// <summary>
        /// Demonstrates how to set off a processor to continually index events
        /// As this is a test which needs to run in a confined time span - this example includes a short circuit
        /// </summary>
        [Fact]
        public async Task RunContinually()
        {
            var web3 = new Web3.Web3(BlockchainUrl);
            var currentBlockNumber = (ulong)(await web3.Eth.Blocks.GetBlockNumber.SendRequestAsync()).Value;
            var startingBlock = currentBlockNumber - 9;
            var maxBlock = currentBlockNumber;

            const ulong expectedBlocks = 10; 

            using (var processor =
                new AzureEventIndexingProcessor(
                    AzureSearchServiceName, _azureSearchApiKey, BlockchainUrl, maxBlocksPerBatch: 1, minBlockConfirmations: 0))
            {
                await ClearDown(processor);

                try
                {
                    await processor.AddAsync<TransferEvent_ERC20>(AzureTransferIndexName);

                    var cancellationToken = new CancellationTokenSource();
                    var shortCircuit = new Action<LogBatchProcessedArgs>((args) =>
                    {
                        if (args.LastRangeProcessed.To.Value >= maxBlock) // escape hatch!
                        {
                            cancellationToken.Cancel();
                        }
                    });

                    var blocksProcessed = await processor.ProcessAsync(startingBlock,
                        ctx: cancellationToken, logBatchProcessedCallback: shortCircuit);

                    Assert.Equal(expectedBlocks, blocksProcessed);
                }
                finally
                {
                    await ClearDown(processor);
                }
            }
        }

        /// <summary>
        /// Using a filter is an efficient way of processing
        /// It means the processor only requests relevant events from the chain
        /// Instead of retrieving all events and evaluating each
        /// </summary>
        [Fact]
        public async Task WithAFilter()
        {
            var filter = new FilterInputBuilder<TransferEvent_ERC20>()
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
                await ClearDown(processor);

                try
                {
                    await processor.AddAsync<TransferEvent_ERC20>(AzureTransferIndexName);

                    var blocksProcessed = await processor.ProcessAsync(3860820, 3860820);

                    Assert.Equal((ulong) 1, blocksProcessed);
                    Assert.Equal(1, processor.Indexers[0].Indexed);
                }
                finally
                {
                    await ClearDown(processor);
                }
            }
        }

        /// <summary>
        /// The class AzureEventIndexingProcessor wraps up quite a few components
        /// This gets you going quick and in many cases will be all you need
        /// However, these components can be used in isolation for flexibility and extensibility
        /// This test demonstrates how to do that
        /// </summary>
        [Fact]
        public async Task UsingTheIndividualComponents()
        {
            TransferMetadata.CurrentChainUrl = BlockchainUrl;

            var web3 =
                new Web3.Web3(BlockchainUrl);

            using (var azureSearchService = new AzureSearchService(AzureSearchServiceName, _azureSearchApiKey))
            {
                await azureSearchService.DeleteIndexAsync(AzureTransferIndexName);

                try
                {
                    using (var transferIndexer =
                        await azureSearchService.CreateEventIndexer<TransferEvent_ERC20>(AzureTransferIndexName))
                    {
                        using (var transferProcessor =
                            new EventIndexProcessor<TransferEvent_ERC20>(transferIndexer))
                        {

                            var logProcessor = new BlockRangeLogsProcessor(
                                web3.Eth.Filters.GetLogs,
                                new ILogProcessor[] {transferProcessor});

                            var progressRepository =
                                new JsonBlockProgressRepository(CreateJsonFileToHoldProgress());

                            var progressService = new StaticBlockRangeProgressService(
                                3146684, 3146694, progressRepository);

                            var batchProcessorService = new LogsProcessor(
                                logProcessor, progressService, maxNumberOfBlocksPerBatch: 2);

                            BlockRange? lastBlockRangeProcessed;
                            do
                            {
                                lastBlockRangeProcessed = await batchProcessorService.ProcessOnceAsync();
                            } while (lastBlockRangeProcessed != null);

                            Assert.Equal(19, transferIndexer.Indexed);
                        }
                    }
                }
                finally
                {
                    await azureSearchService.DeleteIndexAsync(AzureTransferIndexName);
                }
            }
        }

        /// <summary>
        /// Some events look similar but have differing number of indexed parameters
        /// e.g. Transfer is often implemented in slightly different ways
        /// This demonstrates how to index multiple types of different Transfer events in the same search index
        /// </summary>
        [Fact]
        public async Task IndexingTransferEventsWithDifferingSignatures()
        {
            using (var processor =
                new AzureEventIndexingProcessor(AzureSearchServiceName, _azureSearchApiKey, BlockchainUrl))
            {
                await ClearDown(processor);

                try
                {
                    await processor.AddAsync<TransferEvent_With3Indexes>(AzureTransferIndexName);
                    await processor.AddAsync<TransferEvent_ERC20>(AzureTransferIndexName);

                    var blocksProcessed = await processor.ProcessAsync(3146684, 3146694);

                    Assert.Equal((ulong) 11, blocksProcessed);
                    Assert.Equal(2, processor.Indexers.Count);
                    Assert.Equal(6, processor.Indexers[0].Indexed);
                    Assert.Equal(19, processor.Indexers[1].Indexed);

                    await Task.Delay(5000); // leave time for index

                    var customIndexer = processor.Indexers[0] as IIndexer;
                    var erc20Indexer = processor.Indexers[1] as IIndexer;

                    //the indexers wrap the same underlying azure index
                    //this azure index should have documents for both transfer event types
                    Assert.Equal(25, await erc20Indexer.DocumentCountAsync());
                    Assert.Equal(25, await customIndexer.DocumentCountAsync());
                }
                finally
                {
                    await ClearDown(processor);
                }
            }
        }

        /// <summary>
        /// In addition to indexing the data that is the blockchain,
        /// additional data can be indexed.
        /// The data could be derived from the current context, or from custom rules or formulas
        /// This can enrich the search index
        /// </summary>
        [Fact]
        public async Task IndexingAdditionalInformationPerEvent()
        {
            using (var processor =
                new AzureEventIndexingProcessor(AzureSearchServiceName, _azureSearchApiKey, BlockchainUrl))
            {
                await ClearDown(processor);

                try
                {

                    var transferEventProcessor = await processor.AddAsync<TransferEvent_Extended>(AzureTransferIndexName);

                    await processor.ProcessAsync(3146684, 3146694);

                    await Task.Delay(5000); // leave time for index

                    var customIndexer = transferEventProcessor.Indexer as IAzureIndex;

                    var searchByMachineNameQuery = Environment.MachineName;
                    var searchResult = await customIndexer.SearchAsync(searchByMachineNameQuery);

                    Assert.Equal(19, searchResult.Count);

                    //all should have our custom metadata which is not provided by the blockchain
                    foreach (var result in searchResult.Results)
                    {
                        var indexedOnTimestamp = result.Document["metadata_indexedon"];
                        Assert.NotNull(indexedOnTimestamp);
                    }
                }
                finally
                {
                    await ClearDown(processor);
                }
            }
        }

        private static async Task ClearDown(AzureEventIndexingProcessor processor)
        {
            await processor.ClearProgress();
            await processor.SearchService.DeleteIndexAsync(AzureTransferIndexName);
        }

        /// <summary>
        /// An example of a custom DTO based on a transfer event to be stored in the azure index
        /// </summary>
        public class CustomTransferSearchDocumentDto
        {
            public string From { get; set; }
            public string To { get; set; }
            public string Value { get; set; }
            public string BlockNumber { get; set; }
            public string TxHash { get; set; }
            public string LogAddress { get; set; }
            public int LogIndex { get; set; }

            public string DocumentKey { get; set; }
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

        private static Index CreateAzureIndexDefinition()
        {
            var index = new Index { Name = AzureTransferIndexName };
            index.Fields = new List<Field>
                {
                    new Field("DocumentKey", DataType.String) { IsKey = true },
                    new Field("From", DataType.String) { IsSearchable = true, IsFacetable = true },
                    new Field("To", DataType.String) { IsSearchable = true, IsFacetable = true },
                    new Field("Value", DataType.String),
                    new Field("BlockNumber", DataType.String),
                    new Field("TxHash", DataType.String),
                    new Field("LogAddress", DataType.String),
                    new Field("LogIndex", DataType.Int64)
                };
            return index;
        }

    
        private static string CreateJsonFileToHoldProgress()
        {
            var progressFileNameAndPath = Path.Combine(Path.GetTempPath(), "BlockProcess.json");
            if (File.Exists(progressFileNameAndPath)) File.Delete(progressFileNameAndPath);
            return progressFileNameAndPath;
        }

    }
}

