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

            public bool IsLogForEvent(FilterLog log)
            {
                return log.IsLogForEvent<TransferEvent>();
            }

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

            public bool IsLogForEvent(FilterLog log)
            {
                return true;
            }

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

            var progressRepository = new JsonBlockProcessProgressRepository(progressFileNameAndPath);
            var progressService = new PreDefinedRangeBlockchainProcessingProgressService(
                3146684, 3146684, progressRepository);

            var batchProcessorService = new BlockchainBatchProcessorService(
                logProcessor, progressService, maxNumberOfBlocksPerBatch: 1);

            await batchProcessorService.ProcessLatestBlocks();

            Assert.Single(transferEventProcessor.ProcessedEvents);
            Assert.Equal(7, catchAllEventProcessor.ProcessedEvents.Count);

            Assert.Equal((ulong?)3146684, await progressRepository.GetLatestAsync());
            
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

            var progressRepository = new JsonBlockProcessProgressRepository(progressFileNameAndPath);
            var progressService = new PreDefinedRangeBlockchainProcessingProgressService(
                3146684, 3146684, progressRepository);

            var batchProcessorService = new BlockchainBatchProcessorService(
                logProcessor, progressService, maxNumberOfBlocksPerBatch: 1);

            await batchProcessorService.ProcessLatestBlocks();

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
                .AddCondition(eventVal => eventVal.To = "0xc14934679e71ef4d18b6ae927fe2b953c7fd9b91" )
                .Build();

            var logProcessor = new BlockchainLogProcessor(web3Wrapper, eventProcessors, filter);

            var progressFileNameAndPath = Path.Combine(Path.GetTempPath(), "BlockProcess.json");
            if(File.Exists(progressFileNameAndPath)) File.Delete(progressFileNameAndPath);

            var progressRepository = new JsonBlockProcessProgressRepository(progressFileNameAndPath);
            var progressService = new PreDefinedRangeBlockchainProcessingProgressService(
                3146684, 3146684, progressRepository);

            var batchProcessorService = new BlockchainBatchProcessorService(
                logProcessor, progressService, maxNumberOfBlocksPerBatch: 1);

            await batchProcessorService.ProcessLatestBlocks();

            Assert.Single(transferEventProcessor.ProcessedEvents);
            Assert.Equal("0xc14934679e71ef4d18b6ae927fe2b953c7fd9b91", transferEventProcessor.ProcessedEvents.First().Item2.Event.To);
        }

        [Fact]
        public async Task TargetSpecificEventForASpecificContracts()
        {
            var web3Wrapper = new Web3Wrapper("https://rinkeby.infura.io/v3/25e7b6dfc51040b3bfc0e47317d38f60");
            var transferEventProcessor = new TransferEventProcessor();
            var catchAllEventProcessor = new CatchAllEventProcessor();
            var eventProcessors = new ILogProcessor[] {catchAllEventProcessor, transferEventProcessor};

            var ContractAddresses = new []{ "0xC03cDD393C89D169bd4877d58f0554f320f21037"};

            var eventAbi = ABITypedRegistry.GetEvent<TransferEvent>();

            var filter = eventAbi.CreateFilterInput(ContractAddresses);

            var logProcessor = new BlockchainLogProcessor(web3Wrapper, eventProcessors, filter);

            var progressFileNameAndPath = Path.Combine(Path.GetTempPath(), "BlockProcess.json");
            if(File.Exists(progressFileNameAndPath)) File.Delete(progressFileNameAndPath);

            var progressRepository = new JsonBlockProcessProgressRepository(progressFileNameAndPath);
            var progressService = new PreDefinedRangeBlockchainProcessingProgressService(
                3146684, 3146684, progressRepository);

            var batchProcessorService = new BlockchainBatchProcessorService(
                logProcessor, progressService, maxNumberOfBlocksPerBatch: 1);

            await batchProcessorService.ProcessLatestBlocks();

            Assert.Single(transferEventProcessor.ProcessedEvents);
            Assert.Single(catchAllEventProcessor.ProcessedEvents);
        
        }

        [Fact]
        public async Task RunContinually()
        {
            const ulong StartingBlockNumber = 3146684;
            var web3Wrapper = new Web3Wrapper("https://rinkeby.infura.io/v3/25e7b6dfc51040b3bfc0e47317d38f60");

            var transferEventProcessor = new TransferEventProcessor();
            var catchAllEventProcessor = new CatchAllEventProcessor();
            var eventProcessors = new ILogProcessor[] {catchAllEventProcessor, transferEventProcessor};

            var logProcessor = new BlockchainLogProcessor(web3Wrapper, eventProcessors);

            var progressFileNameAndPath = Path.Combine(Path.GetTempPath(), "BlockProcess.json");
            if(File.Exists(progressFileNameAndPath)) File.Delete(progressFileNameAndPath);

            var progressRepository = new JsonBlockProcessProgressRepository(progressFileNameAndPath);

            //this will get the last block on the chain each time a "to" block is requested
            var progressService = new LatestBlockBlockchainProcessingProgressService(
                web3Wrapper, StartingBlockNumber, progressRepository);

            var batchProcessorService = new BlockchainBatchProcessorService(
                logProcessor, progressService, maxNumberOfBlocksPerBatch: 10);

            var iterations = 0;

            //iterate until we reach an arbitrary ending block
            //to process continually - remove the condition from the while loop
            while (progressRepository.Latest < (StartingBlockNumber + 100))
            {
                await batchProcessorService.ProcessLatestBlocks();
                iterations++;
            }

            Assert.Equal(10, iterations);
            Assert.Equal(1533, catchAllEventProcessor.ProcessedEvents.Count);
            Assert.Equal(40, transferEventProcessor.ProcessedEvents.Count);

            //events on other contracts may have same name and input parameter types
            //however they may differ in the number of indexed fields 
            //this leads to decoding errors
            //it's not a problem - just something to be aware of
            Assert.Equal(201, transferEventProcessor.DecodingErrors.Count);
        }
    }
}
