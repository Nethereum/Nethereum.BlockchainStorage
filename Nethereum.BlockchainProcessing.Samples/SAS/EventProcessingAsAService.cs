using Moq;
using Nethereum.BlockchainProcessing.BlockchainProxy;
using Nethereum.BlockchainProcessing.Processing;
using Nethereum.BlockchainProcessing.Processing.Logs;
using Nethereum.BlockchainProcessing.Processing.Logs.Handling;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Nethereum.BlockchainProcessing.Samples.SAS
{
    public class EventProcessingAsAService
    {

        private const string BlockchainUrl = "https://rinkeby.infura.io/v3/25e7b6dfc51040b3bfc0e47317d38f60";

        [Fact]
        public async Task WebJobExample()
        {
            const long PartitionId = 1;
            string JsonProgressFilePath = Path.Combine(Path.GetTempPath(), "WebJobExampleBlockProcess.json");
            if(File.Exists(JsonProgressFilePath)) File.Delete(JsonProgressFilePath);
            const ulong MinimumBlockNumber = 4063361;
            const uint MaxBlocksPerBatch = 10;

            IEventProcessingConfigurationDb configDb = MockEventProcessingDb.CreateMockDb();

            var blockchainProxy = new BlockchainProxyService(new Web3.Web3(BlockchainUrl));
            var eventHandlerFactory = new DecodedEventHandlerFactory(blockchainProxy, configDb);
            var processorFactory = new EventProcessorFactory(configDb, eventHandlerFactory);
            var eventProcessors = await processorFactory.GetLogProcessorsAsync(PartitionId);
            var logProcessor = new BlockchainLogProcessor(blockchainProxy, eventProcessors);
            var jsonProgressRepository = new JsonBlockProgressRepository(JsonProgressFilePath);
            var progressService = new BlockProgressService(blockchainProxy, MinimumBlockNumber, jsonProgressRepository);
            var batchProcessorService = new BlockchainBatchProcessorService(logProcessor, progressService, MaxBlocksPerBatch);

            var ctx = new System.Threading.CancellationTokenSource();
            var rangeProcessed = await batchProcessorService.ProcessLatestBlocksAsync(ctx.Token);

            await eventHandlerFactory.SaveStateAsync();

            Assert.NotNull(rangeProcessed);
            Assert.Equal((ulong)11, rangeProcessed.Value.BlockCount);
        }


    }
}
