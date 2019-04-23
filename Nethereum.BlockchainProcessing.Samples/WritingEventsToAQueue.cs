using Nethereum.ABI.JsonDeserialisation;
using Nethereum.BlockchainProcessing.BlockchainProxy;
using Nethereum.BlockchainProcessing.Processing;
using Nethereum.BlockchainProcessing.Processing.Logs;
using Nethereum.BlockchainProcessing.Processing.Logs.Handling;
using Nethereum.BlockchainProcessing.Processing.Logs.Handling.Handlers;
using Nethereum.BlockchainProcessing.Queue.Azure.Processing.Logs;
using Nethereum.BlockchainStore.AzureTables.Bootstrap;
using Nethereum.RPC.Eth.DTOs;
using System.Threading.Tasks;
using Xunit;

namespace Nethereum.BlockchainProcessing.Samples
{
    public class WritingEventsToAQueue
    {
        private static string MAKER_DAO_ABI = "[{\"constant\":true,\"inputs\":[],\"name\":\"name\",\"outputs\":[{\"name\":\"\",\"type\":\"bytes32\"}],\"payable\":false,\"stateMutability\":\"view\",\"type\":\"function\"},{\"constant\":false,\"inputs\":[],\"name\":\"stop\",\"outputs\":[],\"payable\":false,\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"constant\":false,\"inputs\":[{\"name\":\"guy\",\"type\":\"address\"},{\"name\":\"wad\",\"type\":\"uint256\"}],\"name\":\"approve\",\"outputs\":[{\"name\":\"\",\"type\":\"bool\"}],\"payable\":false,\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"constant\":false,\"inputs\":[{\"name\":\"owner_\",\"type\":\"address\"}],\"name\":\"setOwner\",\"outputs\":[],\"payable\":false,\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"constant\":true,\"inputs\":[],\"name\":\"totalSupply\",\"outputs\":[{\"name\":\"\",\"type\":\"uint256\"}],\"payable\":false,\"stateMutability\":\"view\",\"type\":\"function\"},{\"constant\":false,\"inputs\":[{\"name\":\"src\",\"type\":\"address\"},{\"name\":\"dst\",\"type\":\"address\"},{\"name\":\"wad\",\"type\":\"uint256\"}],\"name\":\"transferFrom\",\"outputs\":[{\"name\":\"\",\"type\":\"bool\"}],\"payable\":false,\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"constant\":true,\"inputs\":[],\"name\":\"decimals\",\"outputs\":[{\"name\":\"\",\"type\":\"uint256\"}],\"payable\":false,\"stateMutability\":\"view\",\"type\":\"function\"},{\"constant\":false,\"inputs\":[{\"name\":\"guy\",\"type\":\"address\"},{\"name\":\"wad\",\"type\":\"uint256\"}],\"name\":\"mint\",\"outputs\":[],\"payable\":false,\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"constant\":false,\"inputs\":[{\"name\":\"wad\",\"type\":\"uint256\"}],\"name\":\"burn\",\"outputs\":[],\"payable\":false,\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"constant\":false,\"inputs\":[{\"name\":\"name_\",\"type\":\"bytes32\"}],\"name\":\"setName\",\"outputs\":[],\"payable\":false,\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"constant\":true,\"inputs\":[{\"name\":\"src\",\"type\":\"address\"}],\"name\":\"balanceOf\",\"outputs\":[{\"name\":\"\",\"type\":\"uint256\"}],\"payable\":false,\"stateMutability\":\"view\",\"type\":\"function\"},{\"constant\":true,\"inputs\":[],\"name\":\"stopped\",\"outputs\":[{\"name\":\"\",\"type\":\"bool\"}],\"payable\":false,\"stateMutability\":\"view\",\"type\":\"function\"},{\"constant\":false,\"inputs\":[{\"name\":\"authority_\",\"type\":\"address\"}],\"name\":\"setAuthority\",\"outputs\":[],\"payable\":false,\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"constant\":true,\"inputs\":[],\"name\":\"owner\",\"outputs\":[{\"name\":\"\",\"type\":\"address\"}],\"payable\":false,\"stateMutability\":\"view\",\"type\":\"function\"},{\"constant\":true,\"inputs\":[],\"name\":\"symbol\",\"outputs\":[{\"name\":\"\",\"type\":\"bytes32\"}],\"payable\":false,\"stateMutability\":\"view\",\"type\":\"function\"},{\"constant\":false,\"inputs\":[{\"name\":\"guy\",\"type\":\"address\"},{\"name\":\"wad\",\"type\":\"uint256\"}],\"name\":\"burn\",\"outputs\":[],\"payable\":false,\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"constant\":false,\"inputs\":[{\"name\":\"wad\",\"type\":\"uint256\"}],\"name\":\"mint\",\"outputs\":[],\"payable\":false,\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"constant\":false,\"inputs\":[{\"name\":\"dst\",\"type\":\"address\"},{\"name\":\"wad\",\"type\":\"uint256\"}],\"name\":\"transfer\",\"outputs\":[{\"name\":\"\",\"type\":\"bool\"}],\"payable\":false,\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"constant\":false,\"inputs\":[{\"name\":\"dst\",\"type\":\"address\"},{\"name\":\"wad\",\"type\":\"uint256\"}],\"name\":\"push\",\"outputs\":[],\"payable\":false,\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"constant\":false,\"inputs\":[{\"name\":\"src\",\"type\":\"address\"},{\"name\":\"dst\",\"type\":\"address\"},{\"name\":\"wad\",\"type\":\"uint256\"}],\"name\":\"move\",\"outputs\":[],\"payable\":false,\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"constant\":false,\"inputs\":[],\"name\":\"start\",\"outputs\":[],\"payable\":false,\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"constant\":true,\"inputs\":[],\"name\":\"authority\",\"outputs\":[{\"name\":\"\",\"type\":\"address\"}],\"payable\":false,\"stateMutability\":\"view\",\"type\":\"function\"},{\"constant\":false,\"inputs\":[{\"name\":\"guy\",\"type\":\"address\"}],\"name\":\"approve\",\"outputs\":[{\"name\":\"\",\"type\":\"bool\"}],\"payable\":false,\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"constant\":true,\"inputs\":[{\"name\":\"src\",\"type\":\"address\"},{\"name\":\"guy\",\"type\":\"address\"}],\"name\":\"allowance\",\"outputs\":[{\"name\":\"\",\"type\":\"uint256\"}],\"payable\":false,\"stateMutability\":\"view\",\"type\":\"function\"},{\"constant\":false,\"inputs\":[{\"name\":\"src\",\"type\":\"address\"},{\"name\":\"wad\",\"type\":\"uint256\"}],\"name\":\"pull\",\"outputs\":[],\"payable\":false,\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"name\":\"symbol_\",\"type\":\"bytes32\"}],\"payable\":false,\"stateMutability\":\"nonpayable\",\"type\":\"constructor\"},{\"anonymous\":false,\"inputs\":[{\"indexed\":true,\"name\":\"guy\",\"type\":\"address\"},{\"indexed\":false,\"name\":\"wad\",\"type\":\"uint256\"}],\"name\":\"Mint\",\"type\":\"event\"},{\"anonymous\":false,\"inputs\":[{\"indexed\":true,\"name\":\"guy\",\"type\":\"address\"},{\"indexed\":false,\"name\":\"wad\",\"type\":\"uint256\"}],\"name\":\"Burn\",\"type\":\"event\"},{\"anonymous\":false,\"inputs\":[{\"indexed\":true,\"name\":\"authority\",\"type\":\"address\"}],\"name\":\"LogSetAuthority\",\"type\":\"event\"},{\"anonymous\":false,\"inputs\":[{\"indexed\":true,\"name\":\"owner\",\"type\":\"address\"}],\"name\":\"LogSetOwner\",\"type\":\"event\"},{\"anonymous\":true,\"inputs\":[{\"indexed\":true,\"name\":\"sig\",\"type\":\"bytes4\"},{\"indexed\":true,\"name\":\"guy\",\"type\":\"address\"},{\"indexed\":true,\"name\":\"foo\",\"type\":\"bytes32\"},{\"indexed\":true,\"name\":\"bar\",\"type\":\"bytes32\"},{\"indexed\":false,\"name\":\"wad\",\"type\":\"uint256\"},{\"indexed\":false,\"name\":\"fax\",\"type\":\"bytes\"}],\"name\":\"LogNote\",\"type\":\"event\"},{\"anonymous\":false,\"inputs\":[{\"indexed\":true,\"name\":\"from\",\"type\":\"address\"},{\"indexed\":true,\"name\":\"to\",\"type\":\"address\"},{\"indexed\":false,\"name\":\"value\",\"type\":\"uint256\"}],\"name\":\"Transfer\",\"type\":\"event\"},{\"anonymous\":false,\"inputs\":[{\"indexed\":true,\"name\":\"owner\",\"type\":\"address\"},{\"indexed\":true,\"name\":\"spender\",\"type\":\"address\"},{\"indexed\":false,\"name\":\"value\",\"type\":\"uint256\"}],\"name\":\"Approval\",\"type\":\"event\"}]";
        public static string MAKER_CONTRACT_ADDRESS = "0x9f8F72aA9304c8B593d555F12eF6589cC3A579A2";
        const ulong MIN_BLOCK_NUMBER = 7540000;
        const uint MAX_BLOCKS_PER_BATCH = 100;

        /// <summary>
        /// Grab all events for the MakerDAO contract and write to an Azure queue
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task QueueAllEventsForMakerDAOContract()
        {
            // Load config
            //  - this will contain the secrets and connection strings we don't want to hard code
            var config = TestConfiguration.LoadConfig();
            string azureStorageConnectionString = config["AzureStorageConnectionString"];

            // Create a proxy for the blockchain
            //  - It will get the logs from the chain
            //  - It provides a log processing friendly abstraction
            //  - Under the hood it wraps the Nethereum.Web3.Web3 object
            //  - In this sample we're hardcoded to target mainnet 
            var blockchainProxy = new BlockchainProxyService(TestConfiguration.BlockchainUrls.Infura.Mainnet);

            // Create Queue Factory 
            //  - In this sample we're targetting Azure
            //  - The factory communicates with Azure to create and get different queues
            var queueFactory = new AzureSubscriberQueueFactory(azureStorageConnectionString);
            // Create a Queue
            //  - This is where we're going to put the matching event logs
            var queue = await queueFactory.GetOrCreateQueueAsync("makerdaoevents");

            //  Get the maker DAO contract abi
            //  - from this we're able to match and decode the events in the contract
            var contractAbi = new ABIDeserialiser().DeserialiseContract(MAKER_DAO_ABI);

            // Create an event subscription for these events
            // - Passing in the maker dao address to ensure only logs with a matching address are processed
            // - There is an option to pass a IEventHandlerHistoryDb implementation (not implemented here)
            // - This would record history for each event handler and is used to prevent duplication
            var eventSubscription = new EventSubscription(contractAbi.Events, new[] { MAKER_CONTRACT_ADDRESS });

            // Assign the queue to the event subscription
            // - Matching events will be written to this queue
            // - By default a generic message is written to the queue
            // - The message contains the raw log (aka FilterLog), decoded event parameter values and event metadata 
            // - Therefore the message schema is consistent across all messages sent to any queues
            // - However - should you require your own queue message schema the method below accepts a custom message mapper
            // - Ultimately the message is converted to json
            eventSubscription.AddQueueHandler(queue);

            // Azure storage setup
            // - this example reads and writes block progress to an Azure storage table
            // - to avoid collision with other samples we provide a prefix
            var storageCloudSetup = new CloudTableSetup(azureStorageConnectionString, prefix: $"makerdao");
           
            // Create a progress repository
            //  - It stores and retrieves the most recent block processed
            var blockProgressRepo = storageCloudSetup.CreateBlockProgressRepository();

            // Create a progress service
            // - This uses the progress repo to dictate what blocks to process next
            // - The MIN_BLOCK_NUMBER dictates the starting point if the progress repo is empty or has fallen too far behind 
            var progressService = new BlockProgressService(blockchainProxy, MIN_BLOCK_NUMBER, blockProgressRepo);

            // Create a filter
            //  - This is essentially the query that is sent to the chain when retrieving logs
            //  - It is OPTIONAL - without it, all logs in the block range are requested
            //  - The filter is invoked before any event subscriptions evaluate the logs
            //  - The subscriptions are free to implement their own matching logic
            //  - In this sample we're only interested in MakerDAO logs
            //  - Therefore it makes sense to restrict the number of logs to retrieve from the chain
            var makerAddressFilter = new NewFilterInput() { Address = new[] { MAKER_CONTRACT_ADDRESS } };

            // Create a log processor
            // - This uses the blockchainProxy to get the logs
            // - It sends each log to the event subscriptions to indicate if the logs matches the subscription criteria
            // - It then allocates matching logs to separate batches per event subscription 
            var logProcessor = new BlockchainLogProcessor(blockchainProxy, new[] { eventSubscription }, makerAddressFilter);

            // Create a batch log processor service
            // - It uses the progress service to calculates the block range to progress
            // - It then invokes the log processor - passing in the range to process
            // - It updates progress via the progress service
            var batchProcessorService = new BlockchainBatchProcessorService(logProcessor, progressService, MAX_BLOCKS_PER_BATCH);

            // execute
            try
            {
                // Optional cancellation token
                // - Useful for cancelling long running processing operations
                var ctx = new System.Threading.CancellationTokenSource();

                // instruct the service to get and process the next range of blocks
                // when the rangeProcessed is null - it means there was nothing to process 
                var rangeProcessed = await batchProcessorService.ProcessLatestBlocksAsync(ctx.Token);

                // ensure we have processed the expected number of events
                // the event subscription has state which can record running totals across many processing batches
                Assert.Equal(16, eventSubscription.State.GetInt("EventsHandled"));
                // get the message count from the queue 
                Assert.Equal(16, await queue.GetApproxMessageCountAsync());
            }
            finally
            {
                // delete any data from Azure
                await ClearDown(queue, storageCloudSetup, queueFactory);
            }

        }

        private async Task ClearDown(
            IQueue queue,
            CloudTableSetup cloudTableSetup,
            AzureSubscriberQueueFactory subscriberQueueFactory)
        {

            var qRef = subscriberQueueFactory.CloudQueueClient.GetQueueReference(queue.Name);
            await qRef.DeleteIfExistsAsync();
            await cloudTableSetup.GetCountersTable().DeleteIfExistsAsync();
        }

    }
}
