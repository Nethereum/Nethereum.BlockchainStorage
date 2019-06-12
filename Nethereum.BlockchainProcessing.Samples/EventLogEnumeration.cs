using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.BlockchainProcessing.Processing;
using Nethereum.BlockchainProcessing.Processing.Logs;
using Nethereum.Contracts;
using Nethereum.Contracts.Extensions;
using Nethereum.RPC.Eth.DTOs;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;
using Nethereum.BlockchainProcessing.BlockchainProxy;
using Xunit;
using Nethereum.LogProcessing;

namespace Nethereum.BlockchainProcessing.Samples
{
    public class EventLogEnumeration
    {
        /*
Solidity Contract Excerpt from ERC721 
* event Transfer(address indexed _from, address indexed _to, uint256 indexed _value);
* Note: all parameters are indexed

Other contracts (e.g. ERC20) may have transfer events where the parameters are indexed differently,
For instance, the third event parameter for ERC20 is not indexed.  
The class below this won't work for those.
The event signature will match (as it "indexed" is not part of the signature) but decoding will fail
*/
        [Event("Transfer")]
        public class TransferEventERC721
        {
            [Parameter(type: "address", name: "_from", order: 1, indexed: true)]
            public string From {get; set;}

            [Parameter(type: "address", name: "_to", order: 2, indexed: true)]
            public string To {get; set;}

            [Parameter(type: "uint256", name: "_tokenId", order: 3, indexed: true)]
            public BigInteger TokenId {get; set;}
        }

        public class TransferEventProcessor : ILogProcessor
        {
            public List<(FilterLog, EventLog<TransferEventERC721>)> ProcessedEvents = new List<(FilterLog, EventLog<TransferEventERC721>)>();
            public List<(FilterLog, Exception)> DecodingErrors = new List<(FilterLog, Exception)>();

            public bool IsLogForEvent(FilterLog log) => log.IsLogForEvent<TransferEventERC721>();

            public Task ProcessLogsAsync(params FilterLog[] eventLogs)
            {
                foreach (var eventLog in eventLogs)
                {
                    try
                    {
                        var eventDto = eventLog.DecodeEvent<TransferEventERC721>();
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
            var web3 = new Web3.Web3(TestConfiguration.BlockchainUrls.Infura.Rinkeby);

            var transferEventProcessor = new TransferEventProcessor();
            var catchAllEventProcessor = new CatchAllEventProcessor();
            var eventProcessors = new ILogProcessor[] {catchAllEventProcessor, transferEventProcessor};

            var logProcessor = new BlockRangeLogsProcessor(web3, eventProcessors);

            var progressFileNameAndPath = Path.Combine(Path.GetTempPath(), "BlockProcess.json");
            if(File.Exists(progressFileNameAndPath)) File.Delete(progressFileNameAndPath);

            var progressRepository = new JsonBlockProgressRepository(progressFileNameAndPath);
            var progressService = new StaticBlockRangeProgressService(
                3146684, 3146684, progressRepository);

            var batchProcessorService = new LogsProcessor(
                logProcessor, progressService, maxNumberOfBlocksPerBatch: 1);

            var rangeProcessed = await batchProcessorService.ProcessOnceAsync();

            Assert.NotNull(rangeProcessed);
            Assert.Equal((ulong?)3146684, rangeProcessed.Value.To);

            Assert.Single(transferEventProcessor.ProcessedEvents);
            Assert.Equal(7, catchAllEventProcessor.ProcessedEvents.Count);
        }

        [Fact]
        public async Task RunContinually()
        {
            var web3 = new Web3.Web3(TestConfiguration.BlockchainUrls.Infura.Rinkeby);

            var catchAllEventProcessor = new CatchAllEventProcessor();
            var transferEventProcessor = new TransferEventProcessor();
            var eventProcessors = new ILogProcessor[] {catchAllEventProcessor, transferEventProcessor};

            var logProcessor = new BlockRangeLogsProcessor(web3, eventProcessors);

            var progressFileNameAndPath = Path.Combine(Path.GetTempPath(), "BlockProcess.json");
            if(File.Exists(progressFileNameAndPath)) File.Delete(progressFileNameAndPath);

            var progressRepository = new JsonBlockProgressRepository(progressFileNameAndPath);

            //this will get the last block on the chain each time a "to" block is requested
            var progressService = new BlockProgressService(
                web3, 3379061, progressRepository, minimumBlockConfirmations: 6);

            var batchProcessorService = new LogsProcessor(
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

            Assert.Equal((ulong)20, blocksProcessed);
            Assert.Equal(2, blockRangesProcessed.Count);
            Assert.Equal(new BlockRange(3379061, 3379070), blockRangesProcessed[0]);
            Assert.Equal(new BlockRange(3379071, 3379080), blockRangesProcessed[1]);
            
            Assert.Equal(350, catchAllEventProcessor.ProcessedEvents.Count);
            Assert.Equal(4, transferEventProcessor.ProcessedEvents.Count);

            //there are Transfer events on other contracts with differing number of indexed fields
            //they can't be decoded into our TransferEvent
            Assert.Equal(42, transferEventProcessor.DecodingErrors.Count);

            
        }


        [Fact]
        public async Task Filtering_By_Many_Values()
        {
            var web3 = new Web3.Web3(TestConfiguration.BlockchainUrls.Infura.Rinkeby);

            var transferEventProcessor = new TransferEventProcessor();
            var eventProcessors = new ILogProcessor[] {transferEventProcessor};

            //create filter to catch multiple from addresses
            //and multiple values
            var filter = new NewFilterInputBuilder<TransferEventERC721>()
                .AddTopic(e => e.From, "0x15829f2c25563481178cc4669b229775c6a49a85")
                .AddTopic(e => e.From, "0x84b1383edee2babfe839b2a177425f0682e679f6")
                .AddTopic(e => e.TokenId, new BigInteger(95))
                .AddTopic(e => e.TokenId, new BigInteger(94))
                .Build();

            var logProcessor = new BlockRangeLogsProcessor(web3, eventProcessors, filter);

            var progressFileNameAndPath = Path.Combine(Path.GetTempPath(), "BlockProcess.json");
            if(File.Exists(progressFileNameAndPath)) File.Delete(progressFileNameAndPath);

            var progressRepository = new JsonBlockProgressRepository(progressFileNameAndPath);

            //this will get the last block on the chain each time a "to" block is requested
            var progressService = new BlockProgressService(
                web3, 3379061, progressRepository, minimumBlockConfirmations: 6);

            var batchProcessorService = new LogsProcessor(
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
                    .Select(e => (int)e.Item2.Event.TokenId)
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
            var web3 = new Web3.Web3(TestConfiguration.BlockchainUrls.Infura.Rinkeby);
            var transferEventProcessor = new TransferEventProcessor();
            var catchAllEventProcessor = new CatchAllEventProcessor();
            var eventProcessors = new ILogProcessor[] {catchAllEventProcessor, transferEventProcessor};

            const string TransferToAddress = "0xc14934679e71ef4d18b6ae927fe2b953c7fd9b91";

            // the "from" address it the first indexed parameter on the event
            // to catch all we pass a null
            const string AnyFromAddress = null;

            // we want the event from any contract - so we pass null for the contract address
            const string AnyContract = null;

            var eventAbi = ABITypedRegistry.GetEvent<TransferEventERC721>();

            //the "to" address is the second indexed parameter on the event
            var filter = eventAbi.CreateFilterInput(AnyContract, AnyFromAddress, TransferToAddress);

            var logProcessor = new BlockRangeLogsProcessor(web3, eventProcessors, filter);

            var progressFileNameAndPath = Path.Combine(Path.GetTempPath(), "BlockProcess.json");
            if(File.Exists(progressFileNameAndPath)) File.Delete(progressFileNameAndPath);

            var progressRepository = new JsonBlockProgressRepository(progressFileNameAndPath);
            var progressService = new StaticBlockRangeProgressService(
                3146684, 3146684, progressRepository);

            var batchProcessorService = new LogsProcessor(
                logProcessor, progressService, maxNumberOfBlocksPerBatch: 1);

            await batchProcessorService.ProcessOnceAsync();

            Assert.Single(transferEventProcessor.ProcessedEvents);
            Assert.Single(catchAllEventProcessor.ProcessedEvents);
        
        }

        [Fact]
        public async Task UsingFilterInputBuilder()
        {
            var web3 = new Web3.Web3(TestConfiguration.BlockchainUrls.Infura.Rinkeby);
            var transferEventProcessor = new TransferEventProcessor();
            var eventProcessors = new ILogProcessor[] {transferEventProcessor};

            var filter = new NewFilterInputBuilder<TransferEventERC721>()
                .AddTopic(eventVal => eventVal.To, "0xc14934679e71ef4d18b6ae927fe2b953c7fd9b91" )
                .Build();

            var logProcessor = new BlockRangeLogsProcessor(web3, eventProcessors, filter);

            var progressFileNameAndPath = Path.Combine(Path.GetTempPath(), "BlockProcess.json");
            if(File.Exists(progressFileNameAndPath)) File.Delete(progressFileNameAndPath);

            var progressRepository = new JsonBlockProgressRepository(progressFileNameAndPath);
            var progressService = new StaticBlockRangeProgressService(
                3146684, 3146684, progressRepository);

            var batchProcessorService = new LogsProcessor(
                logProcessor, progressService, maxNumberOfBlocksPerBatch: 1);

            await batchProcessorService.ProcessOnceAsync();

            Assert.Single(transferEventProcessor.ProcessedEvents);
            Assert.Equal("0xc14934679e71ef4d18b6ae927fe2b953c7fd9b91", transferEventProcessor.ProcessedEvents.First().Item2.Event.To);
        }

        [Fact]
        public async Task TargetSpecificEventForSpecificContracts()
        {
            var web3 = new Web3.Web3(TestConfiguration.BlockchainUrls.Infura.Rinkeby);
            var transferEventProcessor = new TransferEventProcessor();
            var catchAllEventProcessor = new CatchAllEventProcessor();
            var eventProcessors = new ILogProcessor[] {catchAllEventProcessor, transferEventProcessor};

            var ContractAddresses = new []{ "0xC03cDD393C89D169bd4877d58f0554f320f21037"};

            var filter = new NewFilterInputBuilder<TransferEventERC721>().Build(ContractAddresses);

            var logProcessor = new BlockRangeLogsProcessor(web3, eventProcessors, filter);

            var progressFileNameAndPath = Path.Combine(Path.GetTempPath(), "BlockProcess.json");
            if(File.Exists(progressFileNameAndPath)) File.Delete(progressFileNameAndPath);

            var progressRepository = new JsonBlockProgressRepository(progressFileNameAndPath);
            var progressService = new StaticBlockRangeProgressService(
                3146684, 3146684, progressRepository);

            var batchProcessorService = new LogsProcessor(
                logProcessor, progressService, maxNumberOfBlocksPerBatch: 1);

            await batchProcessorService.ProcessOnceAsync();

            Assert.Single(transferEventProcessor.ProcessedEvents);
            Assert.Single(catchAllEventProcessor.ProcessedEvents);
        
        }

    }
}
