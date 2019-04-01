using Moq;
using Nethereum.BlockchainProcessing.BlockchainProxy;
using Nethereum.BlockchainProcessing.Processing;
using Nethereum.BlockchainProcessing.Processing.Logs;
using Nethereum.BlockchainProcessing.Processing.Logs.Handling;
using Nethereum.BlockchainProcessing.Queue.Azure.Processing.Logs;
using Nethereum.Configuration;
using Nethereum.Hex.HexTypes;
using System;
using System.Collections.Generic;
using System.IO;
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
            string azureStorageConnectionString = GetAzureStorageConnectionString();
            string jsonProgressFilePath = GetJsonFileForProgress();

            const long PartitionId = 1;
            const ulong MinimumBlockNumber = 4063361;
            const uint MaxBlocksPerBatch = 10;

            var configRepo = new MockEventProcessingRepository();
            IEventProcessingConfigurationDb configDb = MockEventProcessingDb.Create(configRepo);

            var web3 = new Web3.Web3(TestConfiguration.BlockchainUrls.Infura.Rinkeby);
            var blockchainProxy = new BlockchainProxyService(web3);

            List<DecodedEvent> indexedEvents = new List<DecodedEvent>();
            ISubscriberSearchIndexFactory subscriberSearchIndexFactory = MockSubscriberSearchIndexFactory(indexedEvents);

            // load subscribers and event subscriptions
            var subscriberQueueFactory = new AzureSubscriberQueueFactory(azureStorageConnectionString, configDb);
            var eventHandlerFactory = new EventHandlerFactory(blockchainProxy, configDb, subscriberQueueFactory, subscriberSearchIndexFactory);
            var eventSubscriptionFactory = new EventSubscriptionFactory(configDb, eventHandlerFactory);
            List<IEventSubscription> eventSubscriptions = await eventSubscriptionFactory.LoadAsync(PartitionId);

            // load service
            var logProcessor = new BlockchainLogProcessor(blockchainProxy, eventSubscriptions);
            var jsonProgressRepository = new JsonBlockProgressRepository(jsonProgressFilePath);
            var progressService = new BlockProgressService(blockchainProxy, MinimumBlockNumber, jsonProgressRepository);
            var batchProcessorService = new BlockchainBatchProcessorService(logProcessor, progressService, MaxBlocksPerBatch);

            // execute
            var ctx = new System.Threading.CancellationTokenSource();
            var rangeProcessed = await batchProcessorService.ProcessLatestBlocksAsync(ctx.Token);

            // save event subscription state
            await eventHandlerFactory.SaveStateAsync();

            // assertions
            Assert.NotNull(rangeProcessed);
            Assert.Equal((ulong)11, rangeProcessed.Value.BlockCount);

            var subscriptionState1 = configRepo.GetEventSubscriptionState(eventSubscriptionId: 1); // interested in transfers with contract queries and aggregations
            var subscriptionState2 = configRepo.GetEventSubscriptionState(eventSubscriptionId: 2); // interested in transfers with simple aggregation
            var subscriptionState3 = configRepo.GetEventSubscriptionState(eventSubscriptionId: 3); // interested in any event for a specific address

            Assert.Equal("4009000000002040652615", subscriptionState1.Values["RunningTotalForTransferValue"].ToString());
            Assert.Equal((uint)19, subscriptionState2.Values["CurrentTransferCount"]);

            var txForSpecificAddress = (List<string>)subscriptionState3.Values["AllTransactionHashes"];
            Assert.Equal("0x362bcbc78a5cc6156e8d24d95bee6b8f53d7821083940434d2191feba477ae0e", txForSpecificAddress[0]);
            Assert.Equal("0xe63e9422dedf84d0ce13f9f75ebfd86333ce917b2572925fbdd51b51caf89b77", txForSpecificAddress[1]);

            var blockNumbersForSpecificAddress = (List<HexBigInteger>)subscriptionState3.Values["AllBlockNumbers"];
            Assert.Equal((BigInteger)4063362, blockNumbersForSpecificAddress[0].Value);
            Assert.Equal((BigInteger)4063362, blockNumbersForSpecificAddress[1].Value);

            Assert.Equal(19, indexedEvents.Count);
        }

        private static ISubscriberSearchIndexFactory MockSubscriberSearchIndexFactory(List<DecodedEvent> indexedEvents)
        {
            var mockSearchIndex = new Mock<ISubscriberSearchIndex>();
            mockSearchIndex.Setup(i => i.Index(It.IsAny<DecodedEvent>())).Callback<DecodedEvent>(e => indexedEvents.Add(e)).Returns(Task.CompletedTask);
            var mockSearchIndexFactory = new Mock<ISubscriberSearchIndexFactory>();
            mockSearchIndexFactory.Setup(f => f.GetSubscriberSearchIndexAsync(It.IsAny<long>())).ReturnsAsync(mockSearchIndex.Object);
            var subscriberSearchIndexFactory = mockSearchIndexFactory.Object;
            return subscriberSearchIndexFactory;
        }

        private static string GetJsonFileForProgress()
        {
            string JsonProgressFilePath = Path.Combine(Path.GetTempPath(), "WebJobExampleBlockProcess.json");
            if (File.Exists(JsonProgressFilePath)) File.Delete(JsonProgressFilePath);
            return JsonProgressFilePath;
        }

        private static string GetAzureStorageConnectionString()
        {
            ConfigurationUtils.SetEnvironment("development");

            //use the command line to set your azure search api key
            //e.g. dotnet user-secrets set "AzureStorageConnectionString" "<put key here>"
            var appConfig = ConfigurationUtils
                .Build(Array.Empty<string>(), userSecretsId: "Nethereum.BlockchainProcessing.Samples");

            var azureStorageConnectionString = appConfig["AzureStorageConnectionString"];
            return azureStorageConnectionString;
        }
    }
}
