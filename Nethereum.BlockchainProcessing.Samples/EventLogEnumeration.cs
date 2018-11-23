using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.BlockchainProcessing.Processing;
using Nethereum.BlockchainProcessing.Processing.Logs;
using Nethereum.BlockchainProcessing.Web3Abstractions;
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
using Xunit;

namespace Nethereum.BlockchainProcessing.Samples
{
    public class EventLogEnumeration
    {
        /*
Solidity Contract Excerpt
* event Transfer(address indexed _from, address indexed _to, uint256 indexed _value);
Other contracts may have transfer events with different signatures, this won't work for those.
*/
        [Event("Transfer")]
        public class TransferEvent
        {
            [Parameter("address", "_from", 1, true)]
            public string From {get; set;}

            [Parameter("address", "_to", 2, true)]
            public string To {get; set;}

            [Parameter("uint256", "_value", 3, true)]
            public BigInteger Value {get; set;}
        }

        public class TransferEventProcessor : ILogProcessor
        {
            public List<(FilterLog, EventLog<TransferEvent>)> ProcessedEvents = new List<(FilterLog, EventLog<TransferEvent>)>();
            public List<(FilterLog, Exception)> DecodingErrors = new List<(FilterLog, Exception)>();

            public bool IsLogForEvent(FilterLog log) => log.IsLogForEvent<TransferEvent>();

            public Task ProcessLogsAsync(params FilterLog[] eventLogs)
            {
                foreach (var eventLog in eventLogs)
                {
                    try
                    {
                        var eventDto = eventLog.DecodeEvent<TransferEvent>();
                        ProcessedEvents.Add((eventLog, eventDto));
           
                    }
                    catch (Exception ex)
                    {
                        DecodingErrors.Add((eventLog, ex));
                    }
                }

                return Task.CompletedTask;
            }
        }

        public class CatchAllEventProcessor : ILogProcessor
        {
            public List<FilterLog> ProcessedEvents = new List<FilterLog>();

            public bool IsLogForEvent(FilterLog log) => true;

            public Task ProcessLogsAsync(params FilterLog[] eventLogs)
            {
                ProcessedEvents.AddRange(eventLogs);
                return Task.CompletedTask;
            }
        }

        [Fact]
        public async Task RunOnce()
        {
            var web3Wrapper = new Web3Wrapper("https://rinkeby.infura.io/v3/25e7b6dfc51040b3bfc0e47317d38f60");

            var transferEventProcessor = new TransferEventProcessor();
            var catchAllEventProcessor = new CatchAllEventProcessor();
            var eventProcessors = new ILogProcessor[] {catchAllEventProcessor, transferEventProcessor};

            var logProcessor = new BlockchainLogProcessor(web3Wrapper, eventProcessors);

            var progressFileNameAndPath = Path.Combine(Path.GetTempPath(), "BlockProcess.json");
            if(File.Exists(progressFileNameAndPath)) File.Delete(progressFileNameAndPath);

            var progressRepository = new JsonBlockProgressRepository(progressFileNameAndPath);
            var progressService = new StaticBlockRangeProgressService(
                3146684, 3146684, progressRepository);

            var batchProcessorService = new BlockchainBatchProcessorService(
                logProcessor, progressService, maxNumberOfBlocksPerBatch: 1);

            var rangeProcessed = await batchProcessorService.ProcessLatestBlocksAsync();

            Assert.NotNull(rangeProcessed);
            Assert.Equal((ulong?)3146684, rangeProcessed.Value.To);

            Assert.Single(transferEventProcessor.ProcessedEvents);
            Assert.Equal(7, catchAllEventProcessor.ProcessedEvents.Count);
        }

        [Fact]
        public async Task RunContinually()
        {
            var web3Wrapper = new Web3Wrapper("https://rinkeby.infura.io/v3/25e7b6dfc51040b3bfc0e47317d38f60");

            var catchAllEventProcessor = new CatchAllEventProcessor();
            var transferEventProcessor = new TransferEventProcessor();
            var eventProcessors = new ILogProcessor[] {catchAllEventProcessor, transferEventProcessor};

            var logProcessor = new BlockchainLogProcessor(web3Wrapper, eventProcessors);

            var progressFileNameAndPath = Path.Combine(Path.GetTempPath(), "BlockProcess.json");
            if(File.Exists(progressFileNameAndPath)) File.Delete(progressFileNameAndPath);

            var progressRepository = new JsonBlockProgressRepository(progressFileNameAndPath);

            //this will get the last block on the chain each time a "to" block is requested
            var progressService = new BlockProgressService(
                web3Wrapper, 3379061, progressRepository)
            {
                MinimumBlockConfirmations = 6 //stay within x blocks of the most recent
            };

            var batchProcessorService = new BlockchainBatchProcessorService(
                logProcessor, progressService, maxNumberOfBlocksPerBatch: 10);

            var cancellationTokenSource = new CancellationTokenSource();

            var blockRangesProcessed = new List<BlockRange>();

            var rangesProcessedCallback = new Action<uint, BlockRange>((countOfRangesProcessed, lastRange) => 
            {  
                blockRangesProcessed.Add(lastRange);

                // short circuit - something to trigger the cancellation token
                if (countOfRangesProcessed == 2) cancellationTokenSource.Cancel();
            });

            var blocksProcessed = await batchProcessorService.ProcessContinuallyAsync(
                cancellationTokenSource.Token, rangesProcessedCallback);

            Assert.Equal((ulong)22, blocksProcessed);
            Assert.Equal(2, blockRangesProcessed.Count);
            Assert.Equal(new BlockRange(3379061, 3379071), blockRangesProcessed[0]);
            Assert.Equal(new BlockRange(3379072, 3379082), blockRangesProcessed[1]);
            
            Assert.Equal(395, catchAllEventProcessor.ProcessedEvents.Count);
            Assert.Equal(4, transferEventProcessor.ProcessedEvents.Count);

            //there are Transfer events on other contracts with differing number of indexed fields
            //they can't be decoded into our TransferEvent
            Assert.Equal(46, transferEventProcessor.DecodingErrors.Count);

            
        }


        [Fact]
        public async Task Filtering_By_Many_Values()
        {
            var web3Wrapper = new Web3Wrapper("https://rinkeby.infura.io/v3/25e7b6dfc51040b3bfc0e47317d38f60");

            var transferEventProcessor = new TransferEventProcessor();
            var eventProcessors = new ILogProcessor[] {transferEventProcessor};

            //create filter to catch multiple from addresses
            //and multiple values
            var filter = new NewFilterInputBuilder<TransferEvent>()
                .AddTopic(e => e.From, "0x15829f2c25563481178cc4669b229775c6a49a85")
                .AddTopic(e => e.From, "0x84b1383edee2babfe839b2a177425f0682e679f6")
                .AddTopic(e => e.Value, new BigInteger(95))
                .AddTopic(e => e.Value, new BigInteger(94))
                .Build();

            var logProcessor = new BlockchainLogProcessor(web3Wrapper, eventProcessors, filter);

            var progressFileNameAndPath = Path.Combine(Path.GetTempPath(), "BlockProcess.json");
            if(File.Exists(progressFileNameAndPath)) File.Delete(progressFileNameAndPath);

            var progressRepository = new JsonBlockProgressRepository(progressFileNameAndPath);

            //this will get the last block on the chain each time a "to" block is requested
            var progressService = new BlockProgressService(
                web3Wrapper, 3379061, progressRepository)
            {
                MinimumBlockConfirmations = 6 //stay within x blocks of the most recent
            };

            var batchProcessorService = new BlockchainBatchProcessorService(
                logProcessor, progressService, maxNumberOfBlocksPerBatch: 10);

            var cancellationTokenSource = new CancellationTokenSource();

            var blockRangesProcessed = new List<BlockRange>();

            var rangesProcessedCallback = new Action<uint, BlockRange>((countOfRangesProcessed, lastRange) => 
            {  
                blockRangesProcessed.Add(lastRange);

                // short circuit - something to trigger the cancellation token
                if (countOfRangesProcessed == 2) cancellationTokenSource.Cancel();
            });

            await batchProcessorService.ProcessContinuallyAsync(
                cancellationTokenSource.Token, rangesProcessedCallback);

            var distinctFromAddresses =
                transferEventProcessor.ProcessedEvents
                    .Select(e => e.Item2.Event.From)
                    .Distinct()
                    .ToArray();

            var distinctValues =
                transferEventProcessor.ProcessedEvents
                    .Select(e => (int)e.Item2.Event.Value)
                    .Distinct()
                    .ToArray();

            Assert.Equal(2, distinctFromAddresses.Length);
            Assert.Contains("0x15829f2c25563481178cc4669b229775c6a49a85", distinctFromAddresses);
            Assert.Contains("0x84b1383edee2babfe839b2a177425f0682e679f6", distinctFromAddresses);

            Assert.Equal(2, distinctValues.Length);
            Assert.Contains(94, distinctValues);
            Assert.Contains(95, distinctValues);
        }

        [Fact]
        public async Task TargetSpecificEventAndIndexedValueForAnyContract()
        {
            var web3Wrapper = new Web3Wrapper("https://rinkeby.infura.io/v3/25e7b6dfc51040b3bfc0e47317d38f60");
            var transferEventProcessor = new TransferEventProcessor();
            var catchAllEventProcessor = new CatchAllEventProcessor();
            var eventProcessors = new ILogProcessor[] {catchAllEventProcessor, transferEventProcessor};

            const string TransferToAddress = "0xc14934679e71ef4d18b6ae927fe2b953c7fd9b91";

            // the "from" address it the first indexed parameter on the event
            // to catch all we pass a null
            const string AnyFromAddress = null;

            // we want the event from any contract - so we pass null for the contract address
            const string AnyContract = null;

            var eventAbi = ABITypedRegistry.GetEvent<TransferEvent>();

            //the "to" address is the second indexed parameter on the event
            var filter = eventAbi.CreateFilterInput(AnyContract, AnyFromAddress, TransferToAddress);

            var logProcessor = new BlockchainLogProcessor(web3Wrapper, eventProcessors, filter);

            var progressFileNameAndPath = Path.Combine(Path.GetTempPath(), "BlockProcess.json");
            if(File.Exists(progressFileNameAndPath)) File.Delete(progressFileNameAndPath);

            var progressRepository = new JsonBlockProgressRepository(progressFileNameAndPath);
            var progressService = new StaticBlockRangeProgressService(
                3146684, 3146684, progressRepository);

            var batchProcessorService = new BlockchainBatchProcessorService(
                logProcessor, progressService, maxNumberOfBlocksPerBatch: 1);

            await batchProcessorService.ProcessLatestBlocksAsync();

            Assert.Single(transferEventProcessor.ProcessedEvents);
            Assert.Single(catchAllEventProcessor.ProcessedEvents);
        
        }

        [Fact]
        public async Task UsingFilterInputBuilder()
        {
            var web3Wrapper = new Web3Wrapper("https://rinkeby.infura.io/v3/25e7b6dfc51040b3bfc0e47317d38f60");
            var transferEventProcessor = new TransferEventProcessor();
            var eventProcessors = new ILogProcessor[] {transferEventProcessor};

            var filter = new NewFilterInputBuilder<TransferEvent>()
                .AddTopic(eventVal => eventVal.To, "0xc14934679e71ef4d18b6ae927fe2b953c7fd9b91" )
                .Build();

            var logProcessor = new BlockchainLogProcessor(web3Wrapper, eventProcessors, filter);

            var progressFileNameAndPath = Path.Combine(Path.GetTempPath(), "BlockProcess.json");
            if(File.Exists(progressFileNameAndPath)) File.Delete(progressFileNameAndPath);

            var progressRepository = new JsonBlockProgressRepository(progressFileNameAndPath);
            var progressService = new StaticBlockRangeProgressService(
                3146684, 3146684, progressRepository);

            var batchProcessorService = new BlockchainBatchProcessorService(
                logProcessor, progressService, maxNumberOfBlocksPerBatch: 1);

            await batchProcessorService.ProcessLatestBlocksAsync();

            Assert.Single(transferEventProcessor.ProcessedEvents);
            Assert.Equal("0xc14934679e71ef4d18b6ae927fe2b953c7fd9b91", transferEventProcessor.ProcessedEvents.First().Item2.Event.To);
        }

        [Fact]
        public async Task TargetSpecificEventForSpecificContracts()
        {
            var web3Wrapper = new Web3Wrapper("https://rinkeby.infura.io/v3/25e7b6dfc51040b3bfc0e47317d38f60");
            var transferEventProcessor = new TransferEventProcessor();
            var catchAllEventProcessor = new CatchAllEventProcessor();
            var eventProcessors = new ILogProcessor[] {catchAllEventProcessor, transferEventProcessor};

            var ContractAddresses = new []{ "0xC03cDD393C89D169bd4877d58f0554f320f21037"};

            var filter = new NewFilterInputBuilder<TransferEvent>().Build(ContractAddresses);

            var logProcessor = new BlockchainLogProcessor(web3Wrapper, eventProcessors, filter);

            var progressFileNameAndPath = Path.Combine(Path.GetTempPath(), "BlockProcess.json");
            if(File.Exists(progressFileNameAndPath)) File.Delete(progressFileNameAndPath);

            var progressRepository = new JsonBlockProgressRepository(progressFileNameAndPath);
            var progressService = new StaticBlockRangeProgressService(
                3146684, 3146684, progressRepository);

            var batchProcessorService = new BlockchainBatchProcessorService(
                logProcessor, progressService, maxNumberOfBlocksPerBatch: 1);

            await batchProcessorService.ProcessLatestBlocksAsync();

            Assert.Single(transferEventProcessor.ProcessedEvents);
            Assert.Single(catchAllEventProcessor.ProcessedEvents);
        
        }


    }
}
