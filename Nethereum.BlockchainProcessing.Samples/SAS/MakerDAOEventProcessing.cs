using Microsoft.Extensions.Configuration;
using Nethereum.BlockchainProcessing.BlockchainProxy;
using Nethereum.BlockchainProcessing.Processing;
using Nethereum.BlockchainProcessing.Processing.Logs;
using Nethereum.BlockchainProcessing.Processing.Logs.Configuration;
using Nethereum.BlockchainProcessing.Queue.Azure.Processing.Logs;
using Nethereum.BlockchainStore.AzureTables.Bootstrap;
using Nethereum.Configuration;
using Nethereum.LogProcessing;
using Nethereum.RPC.Eth.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Nethereum.BlockchainProcessing.Samples.SAS
{
    public class MakerDAOEventProcessing
    {
        const long PARTITION = 2;
        const ulong MIN_BLOCK_NUMBER = 7540000;
        const uint MAX_BLOCKS_PER_BATCH = 100;

        [Fact]
        public async Task WriteAnyMakerEventToQueue()
        {
            var config = TestConfiguration.LoadConfig();
            string azureStorageConnectionString = config["AzureStorageConnectionString"];

            var configurationContext = MakerDAOEventProcessingConfig.Create(PARTITION, out IdGenerator idGenerator);
            IEventProcessingConfigurationRepository configurationRepository = configurationContext.CreateMockRepository(idGenerator);

            var web3 = new Web3.Web3(TestConfiguration.BlockchainUrls.Infura.Mainnet);

            // queue components
            var queueFactory = new AzureSubscriberQueueFactory(azureStorageConnectionString);

            // load subscribers and event subscriptions
            var eventSubscriptionFactory = new EventSubscriptionFactory(
                web3, configurationRepository, queueFactory);

            List<IEventSubscription> eventSubscriptions = await eventSubscriptionFactory.LoadAsync(PARTITION);

            // progress repo (dictates which block ranges to process next)
            // maintain separate progress per partition via a prefix
            var storageCloudSetup = new CloudTableSetup(azureStorageConnectionString, prefix: $"Partition{PARTITION}");
            var blockProgressRepo = storageCloudSetup.CreateBlockProgressRepository();

            //this ensures we only query the chain for events relating to this contract
            var makerAddressFilter = new NewFilterInput() { Address = new[] { MakerDAOEventProcessingConfig.MAKER_CONTRACT_ADDRESS} };

            // load service
            var progressService = new BlockProgressService(web3, MIN_BLOCK_NUMBER, blockProgressRepo);
            var logProcessor = new BlockRangeLogsProcessor(web3, eventSubscriptions, makerAddressFilter);
            var batchProcessorService = new LogsProcessor(logProcessor, progressService, MAX_BLOCKS_PER_BATCH);

            // execute
            var blockRangesProcessed = new List<BlockRange?>();
            try
            {
                for(var i = 0 ; i < 2; i ++) // 2 batch iterations
                { 
                    var ctx = new System.Threading.CancellationTokenSource();
                    var rangeProcessed = await batchProcessorService.ProcessOnceAsync(ctx.Token);
                    blockRangesProcessed.Add(rangeProcessed);

                    // save event subscription state after each batch
                    await configurationRepository.EventSubscriptionStates.UpsertAsync(eventSubscriptions.Select(s => s.State));
                }
            }
            finally
            {
                await ClearDown(configurationContext, storageCloudSetup, queueFactory);
            }            

            var subscriptionState = await configurationRepository.EventSubscriptionStates.GetAsync(eventSubscriptions[0].Id);
            Assert.Equal(2, (int)subscriptionState.Values["HandlerInvocations"]);
            Assert.Equal(28, (int)subscriptionState.Values["EventsHandled"]);

        }


        private async Task ClearDown(
            EventProcessingConfigContext repo, 
            CloudTableSetup cloudTableSetup, 
            AzureSubscriberQueueFactory subscriberQueueFactory)
        {
            foreach (var queue in repo.SubscriberSearchIndexes)
            {
                var qRef = subscriberQueueFactory.CloudQueueClient.GetQueueReference(queue.Name);
                await qRef.DeleteIfExistsAsync();
            }

            await cloudTableSetup.GetCountersTable().DeleteIfExistsAsync();
        }
    }
}