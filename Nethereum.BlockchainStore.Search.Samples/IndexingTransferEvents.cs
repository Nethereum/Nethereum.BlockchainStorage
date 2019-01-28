using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.BlockchainProcessing.Processing;
using Nethereum.BlockchainProcessing.Processing.Logs;
using Nethereum.Contracts;
using Nethereum.Contracts.Extensions;
using Nethereum.RPC.Eth.DTOs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;
using Nethereum.BlockchainProcessing.BlockchainProxy;
using Nethereum.BlockchainStore.Search.Azure;
using Nethereum.Configuration;
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
            var blockchainProxyService =
                new BlockchainProxyService("https://rinkeby.infura.io/v3/25e7b6dfc51040b3bfc0e47317d38f60");

            using (var azureSearchService = new AzureEventSearchService(AzureSearchServiceName, _azureSearchApiKey))
            {
                await azureSearchService.DeleteIndexAsync(AzureTransferIndexName);

                using (var transferIndexer = await azureSearchService.GetOrCreateIndex<TransferEvent_ERC20>(AzureTransferIndexName))
                {
                    using (var transferProcessor =
                        new EventLogSearchIndexProcessor<TransferEvent_ERC20>(transferIndexer))
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
                var customIndexer = await d.Add(() => searchService.GetOrCreateIndex<TransferEvent_Custom>(AzureTransferIndexName));
                var customProcessor = d.Add(() => new EventLogSearchIndexProcessor<TransferEvent_Custom>(customIndexer));

                //add the erc 20 event -
                //this will use the same azure index as the custom event - it won't be re-created or altered
                //the "value" element will still be indexed in azure
                var erc20Indexer = await d.Add(() => searchService.GetOrCreateIndex<TransferEvent_ERC20>(AzureTransferIndexName));
                var erc20Processor = d.Add(() => new EventLogSearchIndexProcessor<TransferEvent_ERC20>(erc20Indexer));

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

            //search=3770910&searchFields=log_block_number&$count=true&$select=log_block_number,log_transaction_index,from,to,value
        } 


        //[Fact]
        //public async Task RunContinually()
        //{
        //    var blockchainProxyService = new BlockchainProxyService("https://rinkeby.infura.io/v3/25e7b6dfc51040b3bfc0e47317d38f60");

        //    var catchAllEventProcessor = new CatchAllEventProcessor();
        //    var transferEventProcessor = new TransferEventProcessor();
        //    var eventProcessors = new ILogProcessor[] {catchAllEventProcessor, transferEventProcessor};

        //    var logProcessor = new BlockchainLogProcessor(blockchainProxyService, eventProcessors);

        //    var progressFileNameAndPath = Path.Combine(Path.GetTempPath(), "BlockProcess.json");
        //    if(File.Exists(progressFileNameAndPath)) File.Delete(progressFileNameAndPath);

        //    var progressRepository = new JsonBlockProgressRepository(progressFileNameAndPath);

        //    //this will get the last block on the chain each time a "to" block is requested
        //    var progressService = new BlockProgressService(
        //        blockchainProxyService, 3379061, progressRepository)
        //    {
        //        MinimumBlockConfirmations = 6 //stay within x blocks of the most recent
        //    };

        //    var batchProcessorService = new BlockchainBatchProcessorService(
        //        logProcessor, progressService, maxNumberOfBlocksPerBatch: 10);

        //    var cancellationTokenSource = new CancellationTokenSource();

        //    var blockRangesProcessed = new List<BlockRange>();

        //    var rangesProcessedCallback = new Action<uint, BlockRange>((countOfRangesProcessed, lastRange) => 
        //    {  
        //        blockRangesProcessed.Add(lastRange);

        //        // short circuit - something to trigger the cancellation token
        //        if (countOfRangesProcessed == 2) cancellationTokenSource.Cancel();
        //    });

        //    var blocksProcessed = await batchProcessorService.ProcessContinuallyAsync(
        //        cancellationTokenSource.Token, rangesProcessedCallback);

        //    Assert.Equal((ulong)22, blocksProcessed);
        //    Assert.Equal(2, blockRangesProcessed.Count);
        //    Assert.Equal(new BlockRange(3379061, 3379071), blockRangesProcessed[0]);
        //    Assert.Equal(new BlockRange(3379072, 3379082), blockRangesProcessed[1]);

        //    Assert.Equal(395, catchAllEventProcessor.ProcessedEvents.Count);
        //    Assert.Equal(4, transferEventProcessor.ProcessedEvents.Count);

        //    //there are Transfer events on other contracts with differing number of indexed fields
        //    //they can't be decoded into our TransferEvent
        //    Assert.Equal(46, transferEventProcessor.DecodingErrors.Count);


        //}


        //[Fact]
        //public async Task Filtering_By_Many_Values()
        //{
        //    var blockchainProxyService = new BlockchainProxyService("https://rinkeby.infura.io/v3/25e7b6dfc51040b3bfc0e47317d38f60");

        //    var transferEventProcessor = new TransferEventProcessor();
        //    var eventProcessors = new ILogProcessor[] {transferEventProcessor};

        //    //create filter to catch multiple from addresses
        //    //and multiple values
        //    var filter = new NewFilterInputBuilder<TransferEvent>()
        //        .AddTopic(e => e.From, "0x15829f2c25563481178cc4669b229775c6a49a85")
        //        .AddTopic(e => e.From, "0x84b1383edee2babfe839b2a177425f0682e679f6")
        //        .AddTopic(e => e.Value, new BigInteger(95))
        //        .AddTopic(e => e.Value, new BigInteger(94))
        //        .Build();

        //    var logProcessor = new BlockchainLogProcessor(blockchainProxyService, eventProcessors, filter);

        //    var progressFileNameAndPath = Path.Combine(Path.GetTempPath(), "BlockProcess.json");
        //    if(File.Exists(progressFileNameAndPath)) File.Delete(progressFileNameAndPath);

        //    var progressRepository = new JsonBlockProgressRepository(progressFileNameAndPath);

        //    //this will get the last block on the chain each time a "to" block is requested
        //    var progressService = new BlockProgressService(
        //        blockchainProxyService, 3379061, progressRepository)
        //    {
        //        MinimumBlockConfirmations = 6 //stay within x blocks of the most recent
        //    };

        //    var batchProcessorService = new BlockchainBatchProcessorService(
        //        logProcessor, progressService, maxNumberOfBlocksPerBatch: 10);

        //    var cancellationTokenSource = new CancellationTokenSource();

        //    var blockRangesProcessed = new List<BlockRange>();

        //    var rangesProcessedCallback = new Action<uint, BlockRange>((countOfRangesProcessed, lastRange) => 
        //    {  
        //        blockRangesProcessed.Add(lastRange);

        //        // short circuit - something to trigger the cancellation token
        //        if (countOfRangesProcessed == 2) cancellationTokenSource.Cancel();
        //    });

        //    await batchProcessorService.ProcessContinuallyAsync(
        //        cancellationTokenSource.Token, rangesProcessedCallback);

        //    var distinctFromAddresses =
        //        transferEventProcessor.ProcessedEvents
        //            .Select(e => e.Item2.Event.From)
        //            .Distinct()
        //            .ToArray();

        //    var distinctValues =
        //        transferEventProcessor.ProcessedEvents
        //            .Select(e => (int)e.Item2.Event.Value)
        //            .Distinct()
        //            .ToArray();

        //    Assert.Equal(2, distinctFromAddresses.Length);
        //    Assert.Contains("0x15829f2c25563481178cc4669b229775c6a49a85", distinctFromAddresses);
        //    Assert.Contains("0x84b1383edee2babfe839b2a177425f0682e679f6", distinctFromAddresses);

        //    Assert.Equal(2, distinctValues.Length);
        //    Assert.Contains(94, distinctValues);
        //    Assert.Contains(95, distinctValues);
        //}

        //[Fact]
        //public async Task TargetSpecificEventAndIndexedValueForAnyContract()
        //{
        //    var blockchainProxyService = new BlockchainProxyService("https://rinkeby.infura.io/v3/25e7b6dfc51040b3bfc0e47317d38f60");
        //    var transferEventProcessor = new TransferEventProcessor();
        //    var catchAllEventProcessor = new CatchAllEventProcessor();
        //    var eventProcessors = new ILogProcessor[] {catchAllEventProcessor, transferEventProcessor};

        //    const string TransferToAddress = "0xc14934679e71ef4d18b6ae927fe2b953c7fd9b91";

        //    // the "from" address it the first indexed parameter on the event
        //    // to catch all we pass a null
        //    const string AnyFromAddress = null;

        //    // we want the event from any contract - so we pass null for the contract address
        //    const string AnyContract = null;

        //    var eventAbi = ABITypedRegistry.GetEvent<TransferEvent>();

        //    //the "to" address is the second indexed parameter on the event
        //    var filter = eventAbi.CreateFilterInput(AnyContract, AnyFromAddress, TransferToAddress);

        //    var logProcessor = new BlockchainLogProcessor(blockchainProxyService, eventProcessors, filter);

        //    var progressFileNameAndPath = Path.Combine(Path.GetTempPath(), "BlockProcess.json");
        //    if(File.Exists(progressFileNameAndPath)) File.Delete(progressFileNameAndPath);

        //    var progressRepository = new JsonBlockProgressRepository(progressFileNameAndPath);
        //    var progressService = new StaticBlockRangeProgressService(
        //        3146684, 3146684, progressRepository);

        //    var batchProcessorService = new BlockchainBatchProcessorService(
        //        logProcessor, progressService, maxNumberOfBlocksPerBatch: 1);

        //    await batchProcessorService.ProcessLatestBlocksAsync();

        //    Assert.Single(transferEventProcessor.ProcessedEvents);
        //    Assert.Single(catchAllEventProcessor.ProcessedEvents);

        //}

        //[Fact]
        //public async Task UsingFilterInputBuilder()
        //{
        //    var blockchainProxyService = new BlockchainProxyService("https://rinkeby.infura.io/v3/25e7b6dfc51040b3bfc0e47317d38f60");
        //    var transferEventProcessor = new TransferEventProcessor();
        //    var eventProcessors = new ILogProcessor[] {transferEventProcessor};

        //    var filter = new NewFilterInputBuilder<TransferEvent>()
        //        .AddTopic(eventVal => eventVal.To, "0xc14934679e71ef4d18b6ae927fe2b953c7fd9b91" )
        //        .Build();

        //    var logProcessor = new BlockchainLogProcessor(blockchainProxyService, eventProcessors, filter);

        //    var progressFileNameAndPath = Path.Combine(Path.GetTempPath(), "BlockProcess.json");
        //    if(File.Exists(progressFileNameAndPath)) File.Delete(progressFileNameAndPath);

        //    var progressRepository = new JsonBlockProgressRepository(progressFileNameAndPath);
        //    var progressService = new StaticBlockRangeProgressService(
        //        3146684, 3146684, progressRepository);

        //    var batchProcessorService = new BlockchainBatchProcessorService(
        //        logProcessor, progressService, maxNumberOfBlocksPerBatch: 1);

        //    await batchProcessorService.ProcessLatestBlocksAsync();

        //    Assert.Single(transferEventProcessor.ProcessedEvents);
        //    Assert.Equal("0xc14934679e71ef4d18b6ae927fe2b953c7fd9b91", transferEventProcessor.ProcessedEvents.First().Item2.Event.To);
        //}

        //[Fact]
        //public async Task TargetSpecificEventForSpecificContracts()
        //{
        //    var blockchainProxyService = new BlockchainProxyService("https://rinkeby.infura.io/v3/25e7b6dfc51040b3bfc0e47317d38f60");
        //    var transferEventProcessor = new TransferEventProcessor();
        //    var catchAllEventProcessor = new CatchAllEventProcessor();
        //    var eventProcessors = new ILogProcessor[] {catchAllEventProcessor, transferEventProcessor};

        //    var ContractAddresses = new []{ "0xC03cDD393C89D169bd4877d58f0554f320f21037"};

        //    var filter = new NewFilterInputBuilder<TransferEvent>().Build(ContractAddresses);

        //    var logProcessor = new BlockchainLogProcessor(blockchainProxyService, eventProcessors, filter);

        //    var progressFileNameAndPath = Path.Combine(Path.GetTempPath(), "BlockProcess.json");
        //    if(File.Exists(progressFileNameAndPath)) File.Delete(progressFileNameAndPath);

        //    var progressRepository = new JsonBlockProgressRepository(progressFileNameAndPath);
        //    var progressService = new StaticBlockRangeProgressService(
        //        3146684, 3146684, progressRepository);

        //    var batchProcessorService = new BlockchainBatchProcessorService(
        //        logProcessor, progressService, maxNumberOfBlocksPerBatch: 1);

        //    await batchProcessorService.ProcessLatestBlocksAsync();

        //    Assert.Single(transferEventProcessor.ProcessedEvents);
        //    Assert.Single(catchAllEventProcessor.ProcessedEvents);

        //}


    }
}

