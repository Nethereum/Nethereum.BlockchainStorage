using Microsoft.Extensions.Configuration;
using Nethereum.BlockchainProcessing.BlockchainProxy;
using Nethereum.BlockchainProcessing.Processing;
using Nethereum.BlockchainProcessing.Processing.Logs;
using Nethereum.BlockchainProcessing.Processing.Logs.Configuration;
using Nethereum.BlockchainProcessing.Queue.Azure.Processing.Logs;
using Nethereum.BlockchainStore.AzureTables.Bootstrap;
using Nethereum.BlockchainStore.AzureTables.Factories;
using Nethereum.BlockchainStore.Search.Azure;
using Nethereum.Configuration;
using Nethereum.Hex.HexTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using Xunit;

namespace Nethereum.BlockchainProcessing.Samples.SAS
{
    public class EventProcessingAsAService
    {
        const long PARTITION = 1;
        const ulong MIN_BLOCK_NUMBER = 4063361;
        const uint MAX_BLOCKS_PER_BATCH = 10;
        const string AZURE_SEARCH_SERVICE_NAME = "blockchainsearch";

        [Fact]
        public async Task WebJobExample()
        {
            var config = TestConfiguration.LoadConfig();
            string azureStorageConnectionString = config["AzureStorageConnectionString"];
            string azureSearchKey = config["AzureSearchApiKey"];

            var configurationContext = EventProcessingConfigMock.Create(PARTITION, out IdGenerator idGenerator);
            IEventProcessingConfigurationRepository configurationRepository = configurationContext.CreateMockRepository(idGenerator);

            var web3 = new Web3.Web3(TestConfiguration.BlockchainUrls.Infura.Rinkeby);
            var blockchainProxy = new BlockchainProxyService(web3);

            // search components
            var searchService = new AzureSearchService(serviceName: AZURE_SEARCH_SERVICE_NAME, searchApiKey: azureSearchKey);
            var searchIndexFactory = new AzureSubscriberSearchIndexFactory(searchService);

            // queue components
            var queueFactory = new AzureSubscriberQueueFactory(azureStorageConnectionString);

            // subscriber repository
            var repositoryFactory = new AzureTablesSubscriberRepositoryFactory(azureStorageConnectionString);

            // load subscribers and event subscriptions
            var eventSubscriptionFactory = new EventSubscriptionFactory(
                blockchainProxy, configurationRepository, queueFactory, searchIndexFactory, repositoryFactory);

            List<IEventSubscription> eventSubscriptions = await eventSubscriptionFactory.LoadAsync(PARTITION);

            // progress repo (dictates which block ranges to process next)
            // maintain separate progress per partition via a prefix
            var storageCloudSetup = new CloudTableSetup(azureStorageConnectionString, prefix: $"Partition{PARTITION}");
            var blockProgressRepo = storageCloudSetup.CreateBlockProgressRepository();

            // load service
            var progressService = new BlockProgressService(blockchainProxy, MIN_BLOCK_NUMBER, blockProgressRepo);
            var logProcessor = new BlockchainLogProcessor(blockchainProxy, eventSubscriptions);
            var batchProcessorService = new BlockchainBatchProcessorService(logProcessor, progressService, MAX_BLOCKS_PER_BATCH);

            // execute
            BlockRange? rangeProcessed;
            try
            {
                var ctx = new System.Threading.CancellationTokenSource();
                rangeProcessed = await batchProcessorService.ProcessLatestBlocksAsync(ctx.Token);
            }
            finally
            {
                await ClearDown(configurationContext, storageCloudSetup, searchService, queueFactory, repositoryFactory);
            }

            // save event subscription state
            await configurationRepository.EventSubscriptionStates.UpsertAsync(eventSubscriptions.Select(s => s.State));
            
            // assertions
            Assert.NotNull(rangeProcessed);
            Assert.Equal((ulong)11, rangeProcessed.Value.BlockCount);

            var subscriptionState1 = configurationContext.GetEventSubscriptionState(eventSubscriptionId: 1); // interested in transfers with contract queries and aggregations
            var subscriptionState2 = configurationContext.GetEventSubscriptionState(eventSubscriptionId: 2); // interested in transfers with simple aggregation
            var subscriptionState3 = configurationContext.GetEventSubscriptionState(eventSubscriptionId: 3); // interested in any event for a specific address

            Assert.Equal("4009000000002040652615", subscriptionState1.Values["RunningTotalForTransferValue"].ToString());
            Assert.Equal((uint)19, subscriptionState2.Values["CurrentTransferCount"]);

            var txForSpecificAddress = (List<string>)subscriptionState3.Values["AllTransactionHashes"];
            Assert.Equal("0x362bcbc78a5cc6156e8d24d95bee6b8f53d7821083940434d2191feba477ae0e", txForSpecificAddress[0]);
            Assert.Equal("0xe63e9422dedf84d0ce13f9f75ebfd86333ce917b2572925fbdd51b51caf89b77", txForSpecificAddress[1]);

            var blockNumbersForSpecificAddress = (List<HexBigInteger>)subscriptionState3.Values["AllBlockNumbers"];
            Assert.Equal((BigInteger)4063362, blockNumbersForSpecificAddress[0].Value);
            Assert.Equal((BigInteger)4063362, blockNumbersForSpecificAddress[1].Value);

        }

        private async Task ClearDown(
            EventProcessingConfigContext repo, 
            CloudTableSetup cloudTableSetup, 
            IAzureSearchService searchService, 
            AzureSubscriberQueueFactory subscriberQueueFactory,
            AzureTablesSubscriberRepositoryFactory azureTablesSubscriberRepositoryFactory)
        {
            foreach(var index in repo.SubscriberSearchIndexes) 
            { 
                await searchService.DeleteIndexAsync(index.Name);
            }

            foreach (var queue in repo.SubscriberSearchIndexes)
            {
                var qRef = subscriberQueueFactory.CloudQueueClient.GetQueueReference(queue.Name);
                await qRef.DeleteIfExistsAsync();
            }

            await cloudTableSetup.GetCountersTable().DeleteIfExistsAsync();

            await azureTablesSubscriberRepositoryFactory.DeleteTablesAsync();
        }

    }
}