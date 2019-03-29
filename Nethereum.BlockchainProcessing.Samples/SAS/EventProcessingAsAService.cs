using Moq;
using Nethereum.BlockchainProcessing.BlockchainProxy;
using Nethereum.BlockchainProcessing.Processing;
using Nethereum.BlockchainProcessing.Processing.Logs;
using Nethereum.BlockchainProcessing.Processing.Logs.Handling;
using Nethereum.Hex.HexTypes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using Xunit;

namespace Nethereum.BlockchainProcessing.Samples.SAS
{
    public class EventProcessingAsAService
    {
        [Fact]
        public async Task WebJobExample()
        {
            const long PartitionId = 1;
            string JsonProgressFilePath = Path.Combine(Path.GetTempPath(), "WebJobExampleBlockProcess.json");
            if(File.Exists(JsonProgressFilePath)) File.Delete(JsonProgressFilePath);
            const ulong MinimumBlockNumber = 4063361;
            const uint MaxBlocksPerBatch = 10;

            var repo = new MockEventProcessingRepository();
            IEventProcessingConfigurationDb configDb = MockEventProcessingDb.Create(repo);

            var web3 = new Web3.Web3(TestConfiguration.BlockchainUrls.Infura.Rinkeby);
            var blockchainProxy = new BlockchainProxyService(web3);
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

            var subscriptionState1 = repo.EventSubscriptionStates[1]; // interested in transfers with contract queries and aggregations
            var subscriptionState2 = repo.EventSubscriptionStates[2]; // interested in transfers with simple aggregation
            var subscriptionState3 = repo.EventSubscriptionStates[3]; // interested in any event for a specific address

            Assert.Equal("4009000000002040652615", subscriptionState1.Values["RunningTotalForTransferValue"].ToString());
            Assert.Equal((uint)19, subscriptionState2.Values["CurrentTransferCount"]);

            var txForSpecificAddress = (List<string>)subscriptionState3.Values["AllTransactionHashes"];
            Assert.Equal("0x362bcbc78a5cc6156e8d24d95bee6b8f53d7821083940434d2191feba477ae0e", txForSpecificAddress[0]);
            Assert.Equal("0xe63e9422dedf84d0ce13f9f75ebfd86333ce917b2572925fbdd51b51caf89b77", txForSpecificAddress[1]);

            var blockNumbersForSpecificAddress = (List<HexBigInteger>)subscriptionState3.Values["AllBlockNumbers"];
            Assert.Equal((BigInteger)4063362, blockNumbersForSpecificAddress[0].Value);
            Assert.Equal((BigInteger)4063362, blockNumbersForSpecificAddress[1].Value);
        }


    }
}
