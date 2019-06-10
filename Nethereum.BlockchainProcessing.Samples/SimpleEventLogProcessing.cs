using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.BlockchainProcessing.Processing.Logs;
using Nethereum.BlockchainProcessing.Queue.Azure.Processing.Logs;
using Nethereum.BlockchainStore.AzureTables.Bootstrap;
using Nethereum.BlockchainStore.Search.Azure;
using Nethereum.Contracts;
using Nethereum.RPC.Eth.DTOs;
using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Nethereum.BlockchainProcessing.Samples
{
    public class SimpleEventLogProcessing
    {
        /// <summary>
        /// Represents a typical ERC20 Transfer Event
        /// </summary>
        [Event("Transfer")]
        public class TransferEventDto : IEventDTO
        {
            [Parameter("address", "_from", 1, true)]
            public string From { get; set; }

            [Parameter("address", "_to", 2, true)]
            public string To { get; set; }

            [Parameter("uint256", "_value", 3, false)]
            public BigInteger Value { get; set; }
        }


        [Event("Approval")]
        public class ApprovalEventDTO : IEventDTO
        {
            [Parameter("address", "_owner", 1, true)]
            public virtual string Owner { get; set; }
            [Parameter("address", "_spender", 2, true)]
            public virtual string Spender { get; set; }
            [Parameter("uint256", "_value", 3, false)]
            public virtual BigInteger Value { get; set; }
        }

        /// <summary>
        /// One contract, one event, minimal setup
        /// </summary>
        [Fact]
        public async Task SubscribingToOneEventOnAContract()
        {
            //cancellation token to enable the listener to be stopped
            //passing in a time limit as a safety valve for the unit test
            var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromMinutes(2));

            //somewhere to put matching events
            //using ConcurrentBag because we'll be referencing the collection on different threads
            var erc20Transfers = new ConcurrentBag<EventLog<TransferEventDto>>();

            //this is the contract we want to listen to
            //the processor also accepts an array of addresses
            const string ContractAddress = "0x9f8F72aA9304c8B593d555F12eF6589cC3A579A2";

            //initialise the processor with a blockchain url
            //contract address or addresses is optional
            //we don't need an account because this is read only
            var processor = 
                new EventLogProcessor(TestConfiguration.BlockchainUrls.Infura.Mainnet, ContractAddress)
                .Configure(c => c.MinimumBlockNumber = 7540000) //optional: default is to start at current block on chain
                .Subscribe<TransferEventDto>((events) => erc20Transfers.AddRange(events)); // transfer events

            //RunInBackgroundAsync does not block the current thread (RunAsync does block)
            var backgroundTask = await processor.RunInBackgroundAsync(cancellationTokenSource.Token);
                
            //simulate doing something else whilst the listener works its magic!
            while (!backgroundTask.IsCompleted)
            {
                if (erc20Transfers.Any())
                {
                    cancellationTokenSource.Cancel();
                    break;
                }
                await Task.Delay(1000);
            }

            Assert.True(erc20Transfers.Any());
        }

        /// <summary>
        /// One contract, one event, minimal setup
        /// </summary>
        [Fact]
        public async Task SubscribingToOneEventOnAContract_v2()
        {
            //cancellation token to enable the listener to be stopped
            //passing in a time limit as a safety valve for the unit test
            var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromMinutes(2));
            //instantiate web3 
            var web3 = new Web3.Web3(TestConfiguration.BlockchainUrls.Infura.Mainnet);

            //somewhere to put matching events
            //using ConcurrentBag because we'll be referencing the collection on different threads
            var erc20Transfers = new ConcurrentBag<EventLog<TransferEventDto>>();

            //this is the contract we want to listen to
            //the processor also accepts an array of addresses
            const string ContractAddress = "0x9f8F72aA9304c8B593d555F12eF6589cC3A579A2";

            //build a processor based on event and contract address
            //pass in a lambda to handle the events 
            var processor = web3.Eth.LogsProcessor<TransferEventDto>(ContractAddress)
                .OnEvents((transfers) => erc20Transfers.AddRange(transfers))
                .SetMinimumBlockNumber(7540000)
                .Build();

            var eventSpecificProcessor = web3.Eth.LogsProcessor<TransferEventDto>()
                .OnEvents((transfers) => { })
                .SetMinimumBlockNumber(10)
                .SetBlocksPerBatch(1)
                .Build();

            var nonEventSpecificProcessor = web3.Eth.LogsProcessor()
                .Add((logs) => { }) //any FilterLogs
                .SetMinimumBlockNumber(10)
                .SetBlocksPerBatch(1)
                .Build();
            
            var filters = new [] { 
                new NewFilterInputBuilder<TransferEventDto>().Build(), 
                new NewFilterInputBuilder<ApprovalEventDTO>().Build() 
            };

            var manyEventsProcessor = web3.Eth.LogsProcessor(filters)
                .Add((logs) => { }) //all (FilterLogs)
                .Add<TransferEventDto>((transfers) => { })
                .Add<ApprovalEventDTO>((approvals) => { })
                .Build();

            //event and topic specific
            var eventAndTopicProcesor = web3.Eth.LogsProcessor<TransferEventDto>((f) => f.AddTopic(t => t.From, "xyz"))
                .OnEvents((transfers) => { })
                .Build();

            //events on a contract
            var contractEventsProcessor = web3.Eth.LogsProcessor(ContractAddress)
                .Add<ApprovalEventDTO>(approvals => { })
                .Add<TransferEventDto>(transfers => { })
                .Build();

            //events on many contracts
            var manyContractEventsProcessor = web3.Eth.LogsProcessor(new []{ContractAddress })
                .Add<ApprovalEventDTO>(approvals => { })
                .Add<TransferEventDto>(transfers => { })
                .Build();


            //run the processor in the background
            var backgroundTask = processor.ProcessContinuallyInBackgroundAsync(cancellationTokenSource.Token);

            //simulate doing something else whilst the listener works its magic!
            while (!backgroundTask.IsCompleted)
            {
                if (erc20Transfers.Any())
                {
                    cancellationTokenSource.Cancel();
                    break;
                }
                await Task.Delay(1000);
            }

            Assert.True(erc20Transfers.Any());
        }

        /// <summary>
        /// One contract, many events, more advanced setup, running in the background
        /// </summary>
        [Fact]
        public async Task SubscribingToMultipleEventsOnAContract()
        {
            //cancellation token to enable the listener to be stopped
            //passing in a time limit as a safety valve for the unit test
            var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromMinutes(2));

            //somewhere to put matching events
            //using ConcurrentBag because we'll be referencing the collection on different threads
            var erc20Transfers = new ConcurrentBag<EventLog<TransferEventDto>>();
            var approvals = new ConcurrentBag<EventLog<ApprovalEventDTO>>();
            var all = new ConcurrentBag<FilterLog>();

            //capture a fatal exception here (we're not expecting one!)
            Exception fatalException = null;

            //this is the contract we want to listen to
            //the processor also accepts an array of addresses
            const string ContractAddress = "0x9f8F72aA9304c8B593d555F12eF6589cC3A579A2";

            //initialise the processor
            //contract address or addresses is optional
            //we don't need an account because this is read only
            var processor = new EventLogProcessor(TestConfiguration.BlockchainUrls.Infura.Mainnet, ContractAddress)
                .Configure(c => c.MinimumBlockNumber = 7540000) //optional: default is to start at current block on chain
                .Configure(c => c.MaximumBlocksPerBatch = 100) //optional: number of blocks to scan at once, default is 100 
                .CatchAll((events) => all.AddRange(events)) // any event for the contract/s - useful for logging
                .Subscribe<TransferEventDto>((events) => erc20Transfers.AddRange(events)) // transfer events
                .Subscribe<ApprovalEventDTO>((events) => approvals.AddRange(events)) // approval events
                // optional: a handler for a fatal error which would stop processing
                .OnFatalError((ex) => fatalException = ex)
                // for test purposes we'll cancel after a batch or block range has been processed
                // setting this is optional but is useful for monitoring progress
                .OnBatchProcessed((batchesProcessedCount, lastBlockRange) => cancellationTokenSource.Cancel());

            // begin processing
            var backgroundTask = await processor.RunInBackgroundAsync(cancellationTokenSource.Token);

            //simulate doing something else whilst the listener works its magic!
            while (!backgroundTask.IsCompleted)
            {
                await Task.Delay(1000);
            }

            Assert.True(backgroundTask.IsCanceled);
            Assert.Equal(11, erc20Transfers.Count);
            Assert.Equal(5, approvals.Count);
            Assert.Equal(16, all.Count);
            Assert.Null(fatalException);            
        }



        /// <summary>
        /// One event,  many contracts, minimal setup, running in the background
        /// </summary>
        [Fact]
        public async Task SubscribingToOneEventOnManyContracts()
        {
            //cancellation token to enable the listener to be stopped
            //passing in a time limit as a safety valve for the unit test
            var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromMinutes(2));

            //somewhere to put matching events
            //using ConcurrentBag because we'll be referencing the collection on different threads
            var erc20Transfers = new ConcurrentBag<EventLog<TransferEventDto>>();

            //this is the contract we want to listen to
            //the processor also accepts an array of addresses
            var ContractAddresses = new[] { "0x9f8F72aA9304c8B593d555F12eF6589cC3A579A2", "0xa15c7ebe1f07caf6bff097d8a589fb8ac49ae5b3" };

            //initialise the processor
            //contract address or addresses is optional
            //we don't need an account because this is read only
            var processor = new EventLogProcessor(TestConfiguration.BlockchainUrls.Infura.Mainnet, ContractAddresses)
                .Configure(c => c.MinimumBlockNumber = 7540000) //optional: default is to start at current block on chain
                .Subscribe<TransferEventDto>((events) => erc20Transfers.AddRange(events)); // transfer events

            // begin processing in the background
            var backgroundTask = await processor.RunInBackgroundAsync(cancellationTokenSource.Token);

            //simulate doing something else whilst the listener works its magic!
            while (!backgroundTask.IsCompleted)
            {
                if (erc20Transfers.Any())
                {
                    cancellationTokenSource.Cancel();
                    break;
                }
                await Task.Delay(1000);
            }

            Assert.True(erc20Transfers.Any());
        }

        /// <summary>
        /// Any ERC20 transfer event on any contract, minimal setup
        /// Running as a blocking process (NOT in the background)
        /// Demonstrates using a Filter to improve log retrieval performance
        /// </summary>
        [Fact]
        public async Task SubscribingToAnEventOnAnyContract()
        {
            //cancellation token to enable the listener to be stopped
            //passing in a time limit as a safety valve for the unit test
            var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromMinutes(1));

            //somewhere to put matching events
            //using ConcurrentBag because we'll be referencing the collection on different threads
            var erc20Transfers = new ConcurrentBag<EventLog<TransferEventDto>>();

            //initialise the processor with a blockchain url
            var processor = new EventLogProcessor(TestConfiguration.BlockchainUrls.Infura.Mainnet)
                .Configure(c => c.MaximumBlocksPerBatch = 1) //optional: restrict batches to one block at a time
                .Configure(c => c.MinimumBlockNumber = 7540102) //optional: default is to start at current block on chain
                //this example is not limited by contract addresses
                //therefore - so far - so we have no filters
                //without filters every event log for the block range will be retrieved from the chain and evaluated by the subscribers
                //so - to improve performance - we add a filter to ensure only transfer events are retrieved for evaulation
                .Filter<TransferEventDto>() 
                .Subscribe<TransferEventDto>((events) => erc20Transfers.AddRange(events)) // subscribe to transfer events
                // for test purposes we'll stop after processing a batch
                .OnBatchProcessed((rangeCountProcessedSoFar, lastBlockRange) => cancellationTokenSource.Cancel());

            // run continually until cancellation token is fired
            var rangesProcessed = await processor.RunAsync(cancellationTokenSource.Token);

            Assert.True(erc20Transfers.Any());
            Assert.Equal((ulong)1, rangesProcessed);
        }

        /// <summary>
        /// Demonstrates how to use a progress repository with the processor
        /// This stores the block progress so that the processor can be restarted
        /// And pick up where it left off
        /// </summary>
        [Fact]
        public async Task UsingJsonFileProgressRepository()
        {
            //cancellation token to enable the listener to be stopped
            //passing in a time limit as a safety valve for the unit test
            var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromMinutes(1));

            //somewhere to put matching events
            //using ConcurrentBag because we'll be referencing the collection on different threads
            var erc20Transfers = new ConcurrentBag<EventLog<TransferEventDto>>();

            // define a progress repository
            // this sample uses an out of the box simple json file implementation
            // if you want your own - the IBlockProgressRepository interface is easy to implement
            // the processor will use this repo to define which block to start at and update it after each batch is complete 
            // it can prevent duplicate processing that could occur after a restart
            var jsonFilePath = Path.Combine(Path.GetTempPath(), "EventProcessingBlockProgress.json");

            //initialise the processor
            var processor = new EventLogProcessor(TestConfiguration.BlockchainUrls.Infura.Mainnet)
                .Configure(c => c.MaximumBlocksPerBatch = 1) //optional: restrict batches to one block at a time
                .Configure(c => c.MinimumBlockNumber = 7540102) //optional: default is to start at current block on chain
                .Subscribe<TransferEventDto>((events) => erc20Transfers.AddRange(events)) // transfer events
                // for test purposes we'll stop after processing a batch
                .OnBatchProcessed((rangeCountProcessedSoFar, lastBlockRange) => cancellationTokenSource.Cancel())
                // tell the processor to use a Json File based Block Progress Repository
                // for test purposes only we delete any existing file to ensure we start afresh with no previous state
                .UseJsonFileForBlockProgress(jsonFilePath, deleteExistingFile: true);

            //we should have a BlockProgressRepository
            Assert.NotNull(processor.BlockProgressRepository);
            //there should be no prior progress
            Assert.Null(await processor.BlockProgressRepository.GetLastBlockNumberProcessedAsync());

            //run the processor for a while
            var rangesProcessed = await processor.RunAsync(cancellationTokenSource.Token);

            //the last block processed should have been saved
            Assert.NotNull(await processor.BlockProgressRepository.GetLastBlockNumberProcessedAsync());

            //we should have captured some events
            Assert.True(erc20Transfers.Any());
            //clean up
            File.Delete(jsonFilePath);
        }

        /// <summary>
        /// Demonstrates how to use a progress repository with the processor
        /// This stores the block progress so that the processor can be restarted
        /// and pick up where it left off
        /// </summary>
        [Fact]
        public async Task UsingAzureTableStorageProgressRepository()
        {
            // Requires: Nethereum.BlockchainStore.AzureTables

            // Load config
            //  - this will contain the secrets and connection strings we don't want to hard code
            var config = TestConfiguration.LoadConfig();
            string azureStorageConnectionString = config["AzureStorageConnectionString"];

            //cancellation token to enable the listener to be stopped
            //passing in a time limit as a safety valve for the unit test
            var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromMinutes(1));

            //somewhere to put matching events
            //using ConcurrentBag because we'll be referencing the collection on different threads
            var erc20Transfers = new ConcurrentBag<EventLog<TransferEventDto>>();

            //initialise the processor
            var processor = new EventLogProcessor(TestConfiguration.BlockchainUrls.Infura.Mainnet)
                .Configure(c => c.MaximumBlocksPerBatch = 1) //optional: restrict batches to one block at a time
                .Configure(c => c.MinimumBlockNumber = 7540102) //optional: default is to start at current block on chain
                .Subscribe<TransferEventDto>((events) => erc20Transfers.AddRange(events)) // transfer events
                // for test purposes we'll stop after processing a batch
                .OnBatchProcessed((rangeCountProcessedSoFar, lastBlockRange) => cancellationTokenSource.Cancel())
                // tell the processor to reference an Azure Storage table for block progress
                // this is an extension method from Nethereum.BlockchainStore.AzureTables
                .UseAzureTableStorageForBlockProgress(azureStorageConnectionString, "EventLogProcessingSample");

            //we should have a BlockProgressRepository
            Assert.NotNull(processor.BlockProgressRepository);
            //there should be no prior progress
            Assert.Null(await processor.BlockProgressRepository.GetLastBlockNumberProcessedAsync());

            //run the processor for a while
            var rangesProcessed = await processor.RunAsync(cancellationTokenSource.Token);

            //the last block processed should have been saved
            Assert.NotNull(await processor.BlockProgressRepository.GetLastBlockNumberProcessedAsync());

            //we should have captured some events
            Assert.True(erc20Transfers.Any());
            //clean up
            await new CloudTableSetup(azureStorageConnectionString, "EventLogProcessingSample")
                .GetCountersTable()
                .DeleteIfExistsAsync();
        }

        /// <summary>
        /// Demonstrates how to write events to a queue
        /// </summary>
        [Fact]
        public async Task WritingEventsToAnAzureQueue()
        {
            // Requires: Nethereum.BlockchainProcessing.Queue.Azure

            // Load config
            //  - this will contain the secrets and connection strings we don't want to hard code
            var config = TestConfiguration.LoadConfig();
            string azureStorageConnectionString = config["AzureStorageConnectionString"];

            //cancellation token to enable the listener to be stopped
            //passing in a time limit as a safety valve for the unit test
            var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromMinutes(1));

            //initialise the processor
            using(var processor = await new EventLogProcessor(TestConfiguration.BlockchainUrls.Infura.Mainnet)
                .Configure(c => c.MaximumBlocksPerBatch = 1) //optional: restrict batches to one block at a time
                .Configure(c => c.MinimumBlockNumber = 7540103) //optional: default is to start at current block on chain
                .OnBatchProcessed((rangeCountProcessedSoFar, lastBlockRange) => cancellationTokenSource.Cancel())
                .SubscribeAndQueueAsync<TransferEventDto>(azureStorageConnectionString, "sep-transfers"))
            { 
                //run the processor for a while
                var rangesProcessed = await processor.RunAsync(cancellationTokenSource.Token);
            }

            await Task.Delay(5000); //give azure time to update
            
            //clean up
            var queueFactory = new AzureSubscriberQueueFactory(azureStorageConnectionString);
            var queue = await queueFactory.GetOrCreateQueueAsync("sep-transfers");
            Assert.Equal(13, await queue.GetApproxMessageCountAsync());

            //clean up
            await queueFactory.CloudQueueClient.GetQueueReference(queue.Name).DeleteIfExistsAsync();
        }

        /// <summary>
        /// Demonstrates how to write events to a search index
        /// </summary>
        [Fact]
        public async Task WritingEventsToASearchIndex()
        {
            //Requires: Nethereum.BlockchainStore.Search

            // Load config
            //  - this will contain the secrets and connection strings we don't want to hard code
            var config = TestConfiguration.LoadConfig();
            string ApiKeyName = "AzureSearchApiKey";
            string AzureSearchServiceName = "blockchainsearch";
            var apiKey = config[ApiKeyName];

            //cancellation token to enable the listener to be stopped
            //passing in a time limit as a safety valve for the unit test
            var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromMinutes(1));

            //initialise the processor
            //within "using" block so that the processor cleans up the search resources it creates
            using (var processor = await new EventLogProcessor(TestConfiguration.BlockchainUrls.Infura.Mainnet)
                .Configure(c => c.MaximumBlocksPerBatch = 1) //optional: restrict batches to one block at a time
                .Configure(c => c.MinimumBlockNumber = 7540103) //optional: default is to start at current block on chain
                .OnBatchProcessed((rangeCountProcessedSoFar, lastBlockRange) => cancellationTokenSource.Cancel())
                .AddToSearchIndexAsync<TransferEventDto>(AzureSearchServiceName, apiKey, "sep-transfers"))
            {
                //run the processor for a while
                var rangesProcessed = await processor.RunAsync(cancellationTokenSource.Token);
            }

            await Task.Delay(5000); //give azure time to update
            
            using(var searchService = new AzureSearchService(AzureSearchServiceName, apiKey))
            { 
                Assert.Equal((long)13, await searchService.CountDocumentsAsync("sep-transfers"));
                await searchService.DeleteIndexAsync("sep-transfers");
            }
        }

        /// <summary>
        /// Demonstrates how to write events to Azure table storage
        /// </summary>
        [Fact]
        public async Task WritingEventsToAzureTableStorage()
        {
            // Requires: Nethereum.BlockchainStore.AzureTables

            // Load config
            //  - this will contain the secrets and connection strings we don't want to hard code
            var config = TestConfiguration.LoadConfig();
            string azureStorageConnectionString = config["AzureStorageConnectionString"];

            //cancellation token to enable the processor to be stopped
            //passing in a time limit as a safety valve for the unit test
            var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromMinutes(1));

            //initialise the processor
            using (var processor = new EventLogProcessor(TestConfiguration.BlockchainUrls.Infura.Mainnet)
                .Configure(c => c.MaximumBlocksPerBatch = 1) //optional: restrict batches to one block at a time
                .Configure(c => c.MinimumBlockNumber = 7540103) //optional: default is to start at current block on chain
                // configure this to stop after processing a batch
                .OnBatchProcessed((rangeCountProcessedSoFar, lastBlockRange) => cancellationTokenSource.Cancel())
                // wire up to azure table storage
                .StoreInAzureTable<TransferEventDto>(azureStorageConnectionString, "septransfers"))
            {
                //run the processor for a while
                var rangesProcessed = await processor.RunAsync(cancellationTokenSource.Token);
            }

            var expectedLogs = new
            (string TransactionHash, long LogIndex)[]{
                ( "0xd24d77d48b9c19eb8547fb2b2cbe06ee6cfb72b306785fbb65092c38ea665382", 17),
                ("0xd92620a74a3d656fed316eeed859ec290aff825d9d9ecf587163860ef16546a3",15),
                ("0xc4d5f3ec633b26801be9d3fc53251e6eefcdda9e2d4c3175a68faeb2ff098085",11),
                ("0x7c61ded6b3a36c9b997ea2049eb1b07c890a574a707b9910e79b1a88095f8703",10),
                ("0x2af1b081a6a43c02964e645467df79b3c99f934faf6452592cd21fdbbf29cdff",9),
                ("0xc2480ad47fe5a9ebb1931be91b17a846218e5655a25ef97eea020d0b8b41bc63",7),
                ("0x7b17144d88c4dc9503a72804aa3bb4f32ae4c9941a28e5d88532c85179c638d4",6),
                ("0x7b17144d88c4dc9503a72804aa3bb4f32ae4c9941a28e5d88532c85179c638d4",5),
                ("0x7b17144d88c4dc9503a72804aa3bb4f32ae4c9941a28e5d88532c85179c638d4",4),
                ("0x7d054ad829c2a8bb2152454eedc654220a0d989d77dba751bae652ea8eb3dcf6",3),
                ("0x66f09e21f822bca39b317b9bd67317a7388f76f04c78bc2c74c66e4fb8b1e57e",2),
                ("0xc7099c2e4e02c354ae5d19df1628a0c15c1288693301722a804f19b941b809e8",1),
                ("0x5d824e1e088ef3e78d27166bec59a79e3b6b8dfaaa7559e24e46fdc54805f817",0)
            };

            //verify
            var cloudTableSetup = new CloudTableSetup(azureStorageConnectionString, "septransfers");
            var repo = cloudTableSetup.CreateTransactionLogRepository();
            foreach(var logId in expectedLogs)
            {
                var logFromRepo = await repo.FindByTransactionHashAndLogIndexAsync(logId.TransactionHash, logId.LogIndex);
                Assert.NotNull(logFromRepo);
            }

            //clean up
            await cloudTableSetup.GetTransactionsLogTable().DeleteIfExistsAsync();
        }

        [Fact(Skip ="A deliberately long running test to be run when required (certainly not on CI!)")]
        public async Task LongRunningProcessing()
        {
            int eventsCaught = 0;
            int transfers = 0;
            int approvals = 0;

            var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromHours(1));

            var processor = await new EventLogProcessor(TestConfiguration.BlockchainUrls.Infura.Mainnet)
                .Configure(c => c.MaximumBlocksPerBatch = 1)
                .Configure(c => c.MinimumBlockConfirmations = 6)
                .CatchAll((events) =>  { eventsCaught += events.Count(); Debug.WriteLine($"Events: {eventsCaught}"); }) 
                .Subscribe<TransferEventDto>((events) => { transfers += events.Count(); Debug.WriteLine($"Transfers: {transfers}"); }) 
                .Subscribe<ApprovalEventDTO>((events) => { approvals += events.Count(); Debug.WriteLine($"Approvals: {approvals}"); })                                                                    
                .OnFatalError((ex) => Debug.WriteLine($"Fatal Error: {ex.Message}"))
                .OnBatchProcessed((batchesProcessedCount, lastBlockRange) => 
                    Debug.WriteLine($"Batch Processed. Batches: {batchesProcessedCount}, Last Range: From:{lastBlockRange.From} To{lastBlockRange.To}"))
                .RunAsync(cancellationTokenSource.Token);
        }
    }
}
