using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.BlockchainProcessing.Processing.Logs;
using Nethereum.BlockchainStore.AzureTables.Bootstrap;
using Nethereum.Contracts;
using Nethereum.RPC.Eth.DTOs;
using System;
using System.Collections.Concurrent;
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
            //RunInBackgroundAsync does not block the current thread (RunAsync does block)
            var backgroundTask = await
                new EventLogProcessor(TestConfiguration.BlockchainUrls.Infura.Mainnet, ContractAddress)
                .Configure(c => c.MinimumBlockNumber = 7540000) //optional: default is to start at current block on chain
                .Subscribe<TransferEventDto>((events) => erc20Transfers.AddRange(events)) // transfer events
                .RunInBackgroundAsync(cancellationTokenSource.Token);

            //simulate doing something else whilst the listener works its magic!
            while (!cancellationTokenSource.IsCancellationRequested)
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
            while (!cancellationTokenSource.IsCancellationRequested)
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
            while (!cancellationTokenSource.IsCancellationRequested)
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
        /// And pick up where it left off
        /// </summary>
        [Fact]
        public async Task UsingAzureTableStorageProgressRepository()
        {
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
    }
}
