using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.ABI.JsonDeserialisation;
using Nethereum.BlockchainProcessing.BlockchainProxy;
using Nethereum.BlockchainProcessing.Processing;
using Nethereum.BlockchainProcessing.Processing.Logs;
using Nethereum.BlockchainProcessing.Processing.Logs.Handling;
using Nethereum.BlockchainProcessing.Processing.Logs.Handling.Handlers;
using Nethereum.BlockchainProcessing.Queue.Azure.Processing.Logs;
using Nethereum.BlockchainStore.AzureTables.Bootstrap;
using Nethereum.RPC.Eth.DTOs;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Numerics;
using System.Threading.Tasks;
using Xunit;
using Nethereum.Contracts;
using System.Linq;

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
            // - There is an option to pass an implementation of IEventHandlerHistoryRepository in to the constructor
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
                var rangeProcessed = await batchProcessorService.ProcessOnceAsync(ctx.Token);

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

        /// <summary>
        /// Represents a typical ERC20 Transfer Event
        /// </summary>
        [Event("Transfer")]
        public class TransferEventDto: IEventDTO
        {
            [Parameter("address", "_from", 1, true)]
            public string From { get; set; }

            [Parameter("address", "_to", 2, true)]
            public string To { get; set; }

            [Parameter("uint256", "_value", 3, false)]
            public BigInteger Value { get; set; }
        }

        [Fact]
        public async Task QueueAllTransferEventsForMakerDAO()
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
            var queue = await queueFactory.GetOrCreateQueueAsync("makerdaotransfers");


            // Create an event subscription specifically for ERC20 Transfers
            // - Passing in the maker dao address to ensure only logs with a matching address are processed
            // - There is an option to pass an implementation of IEventHandlerHistoryRepository in to the constructor
            // - This would record history for each event handler and is used to prevent duplication
            var eventSubscription = new EventSubscription<TransferEventDto>(
                contractAddressesToMatch: new[] { MAKER_CONTRACT_ADDRESS });

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
            var storageCloudSetup = new CloudTableSetup(azureStorageConnectionString, prefix: $"makerdaotransfers");

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
                var rangeProcessed = await batchProcessorService.ProcessOnceAsync(ctx.Token);

                // ensure we have processed the expected number of events
                // the event subscription has state which can record running totals across many processing batches
                Assert.Equal(11, eventSubscription.State.GetInt("EventsHandled"));
                // get the message count from the queue 
                Assert.Equal(11, await queue.GetApproxMessageCountAsync());

                //See below to see what a sample message looks like

            }
            finally
            {
                // delete any data from Azure
                await ClearDown(queue, storageCloudSetup, queueFactory);
            }

        }

        [Fact]
        public async Task WritingCustomMessagesToTheQueue()
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
            var queue = await queueFactory.GetOrCreateQueueAsync("makerdaotransferscustom");


            // Create an event subscription specifically for ERC20 Transfers
            // - Passing in the maker dao address to ensure only logs with a matching address are processed
            // - There is an option to pass an implementation of IEventHandlerHistoryRepository in to the constructor
            // - This would record history for each event handler and is used to prevent duplication
            var eventSubscription = new EventSubscription<TransferEventDto>(
                contractAddressesToMatch: new[] { MAKER_CONTRACT_ADDRESS });

            // Create a mapper that will convert the DecodedEvent into a custom message we want on the queue
            // In this sample we're using a subscription that is specific to an EventDTO (EventSubscription<TransferEventDto>) 
            // This ensures that the decodedEvent.DecodedEventDto property is populated during processing
            // ( If the subscription is not tied to an EventDTO the decodedEvent.DecodedEventDto property would be null 
            //   BUT we can still read the event arguments (aka parameters or topics) from the decodedEvent.Event property)
            var queueMessageMapper = new QueueMessageMapper((decodedEvent) =>
            {
                return new CustomQueueMessageForTransfers
                {
                    BlockNumber = decodedEvent.Log.BlockNumber.Value.ToString(),
                    TransactionHash = decodedEvent.Log.TransactionHash,
                    LogIndex = decodedEvent.Log.LogIndex.Value.ToString(),
                    Transfer = decodedEvent.DecodedEventDto as TransferEventDto
                };
            });


            // Assign the queue to the event subscription
            // - Matching events will be written to this queue
            // - Pass a custom mapper to create a suitable queue message
            // - Ultimately the message is converted to json
            eventSubscription.AddQueueHandler(queue, queueMessageMapper);

            // Azure storage setup
            // - this example reads and writes block progress to an Azure storage table
            // - to avoid collision with other samples we provide a prefix
            var storageCloudSetup = new CloudTableSetup(azureStorageConnectionString, prefix: $"makerdaotransferscustom");

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
                var rangeProcessed = await batchProcessorService.ProcessOnceAsync(ctx.Token);

                // ensure we have processed the expected number of events
                // the event subscription has state which can record running totals across many processing batches
                Assert.Equal(11, eventSubscription.State.GetInt("EventsHandled"));
                // get the message count from the queue 
                Assert.Equal(11, await queue.GetApproxMessageCountAsync());

                //A sample message body from the queue
                /*
                 * {"BlockNumber":"7540010","TransactionHash":"0x8d58abc578f5e321f2e6b7c0637ccc60fbf62b39b120691cbf19ff201f5069b0","LogIndex":"132","Transfer":{"From":"0x296c61eaf5bea208bbabc65ae01c3bc5270fe386","To":"0x2a8f1a6af55b705b7daee0776d6f97302de2a839","Value":119928660890733235}}
                 */
            }
            finally
            {
                // delete any data from Azure
                await ClearDown(queue, storageCloudSetup, queueFactory);
            }

        }

        /// <summary>
        /// A very simple DTO to represent something you may want on the queue
        /// In this case it represents a Transfer
        /// </summary>
        public class CustomQueueMessageForTransfers
        {
            public string BlockNumber { get;set;}
            public string TransactionHash { get;set;}
            public string LogIndex { get;set;}

            public TransferEventDto Transfer { get;set;}
        }



        [Fact]
        public void DecodingAQueueMessage()
        {
            // How to deserialize a message from the queue

            // IMPORTANT
            // This assumes the message being written to the queue is the default message
            // (There is a non default option to write a custom message to the queue instead)

            // sample queue message body for a transfer event
            var eventMessageJson = "{\"Key\":\"7540010_0x8d58abc578f5e321f2e6b7c0637ccc60fbf62b39b120691cbf19ff201f5069b0_132\",\"Event\":{\"From\":\"0x296c61eaf5bea208bbabc65ae01c3bc5270fe386\",\"To\":\"0x2a8f1a6af55b705b7daee0776d6f97302de2a839\",\"Value\":119928660890733235},\"State\":{\"EventAbiName\":\"Transfer\",\"EventSignature\":\"ddf252ad1be2c89b69c2b068fc378daa952ba7f163c4a11628f55a4df523b3ef\",\"TransactionHash\":\"0x8d58abc578f5e321f2e6b7c0637ccc60fbf62b39b120691cbf19ff201f5069b0\",\"LogIndex\":132,\"HandlerInvocations\":1,\"UtcNowMs\":1556872537180,\"SubscriberId\":0,\"EventSubscriptionId\":0},\"Transaction\":null,\"ParameterValues\":[{\"Order\":1,\"Name\":\"_from\",\"AbiType\":\"address\",\"Value\":\"0x296c61eaf5bea208bbabc65ae01c3bc5270fe386\",\"Indexed\":true},{\"Order\":2,\"Name\":\"_to\",\"AbiType\":\"address\",\"Value\":\"0x2a8f1a6af55b705b7daee0776d6f97302de2a839\",\"Indexed\":true},{\"Order\":3,\"Name\":\"_value\",\"AbiType\":\"uint256\",\"Value\":119928660890733235,\"Indexed\":false}],\"Log\":{\"removed\":false,\"type\":null,\"logIndex\":\"0x84\",\"transactionHash\":\"0x8d58abc578f5e321f2e6b7c0637ccc60fbf62b39b120691cbf19ff201f5069b0\",\"transactionIndex\":\"0x95\",\"blockHash\":\"0x0ef49c4e3a11872084715808df7a07c4fa6f03dac0596b4e6c10b21b88f7d227\",\"blockNumber\":\"0x730d2a\",\"address\":\"0x9f8f72aa9304c8b593d555f12ef6589cc3a579a2\",\"data\":\"0x00000000000000000000000000000000000000000000000001aa127b4ec7cab3\",\"topics\":[\"0xddf252ad1be2c89b69c2b068fc378daa952ba7f163c4a11628f55a4df523b3ef\",\"0x000000000000000000000000296c61eaf5bea208bbabc65ae01c3bc5270fe386\",\"0x0000000000000000000000002a8f1a6af55b705b7daee0776d6f97302de2a839\"]}}";

            //OPTION 1
            // Deserialize the message from the queue into a generic json object
            // Deserialize the Log property into a FilterLog
            // Decode the FilterLog into the Event DTO
            // Requires reference to Nethereum.Web3
            // Does NOT require reference to Nethereum.BlockchainProcessing

            var jObject = (JObject)JsonConvert.DeserializeObject(eventMessageJson);
            var log = jObject["Log"].ToObject<FilterLog>();
            var transferEvent = log.DecodeEvent<TransferEventDto>();

            //we have the info from the log
            Assert.Equal("0x8d58abc578f5e321f2e6b7c0637ccc60fbf62b39b120691cbf19ff201f5069b0", transferEvent.Log.TransactionHash);

            //we have the event parameter values
            Assert.Equal("0x296c61eaf5bea208bbabc65ae01c3bc5270fe386", transferEvent.Event.From);
            Assert.Equal("0x2a8f1a6af55b705b7daee0776d6f97302de2a839", transferEvent.Event.To);
            Assert.Equal(new BigInteger(119928660890733235), transferEvent.Event.Value);

            // OPTION 2
            // Deserialize to an EventLogQueueMessage 
            // Deserializing into the same object that was serialized to the queue
            // A generic object that represents an event
            // Requires reference to Nethereum.Web3
            // Requires reference to Nethereum.BlockchainProcessing

            var queueMessage = JsonConvert.DeserializeObject<EventLogQueueMessage>(eventMessageJson);

            //we have the info from the log
            Assert.Equal("0x8d58abc578f5e321f2e6b7c0637ccc60fbf62b39b120691cbf19ff201f5069b0", queueMessage.Log.TransactionHash);

            // we have the decoded event parameter values
            Assert.Equal("0x296c61eaf5bea208bbabc65ae01c3bc5270fe386", queueMessage.ParameterValues[0].Value);
            Assert.Equal("0x2a8f1a6af55b705b7daee0776d6f97302de2a839", queueMessage.ParameterValues[1].Value);
            Assert.Equal(new BigInteger(119928660890733235), new BigInteger((long)queueMessage.ParameterValues[2].Value));

            //we could also decode the log into the transfer event dto
            transferEvent = queueMessage.Log.DecodeEvent<TransferEventDto>();
            Assert.Equal("0x296c61eaf5bea208bbabc65ae01c3bc5270fe386", transferEvent.Event.From);
            Assert.Equal("0x2a8f1a6af55b705b7daee0776d6f97302de2a839", transferEvent.Event.To);
            Assert.Equal(new BigInteger(119928660890733235), transferEvent.Event.Value);

            // OPTION 4
            // This is only relevant if the event subscription writing the message to the queue is specific to an Event DTO
            // i.e. you are using the generic EventSubscription<EventDTO> class
            // Whilst processing the log is decoded into an EventDTO instance and assigned to the message

            queueMessage = JsonConvert.DeserializeObject<EventLogQueueMessage>(eventMessageJson);

            //we have the info from the log
            Assert.Equal("0x8d58abc578f5e321f2e6b7c0637ccc60fbf62b39b120691cbf19ff201f5069b0", queueMessage.Log.TransactionHash);

            // we have the decoded event parameter values
            Assert.Equal("0x296c61eaf5bea208bbabc65ae01c3bc5270fe386", queueMessage.ParameterValues[0].Value);
            Assert.Equal("0x2a8f1a6af55b705b7daee0776d6f97302de2a839", queueMessage.ParameterValues[1].Value);
            Assert.Equal(new BigInteger(119928660890733235), new BigInteger((long)queueMessage.ParameterValues[2].Value));

            //we have a populated object with the dto
            Assert.NotNull(queueMessage.Event);
            //the json derializer assigns a jObject as the specific dto type is not defined by the message class
            Assert.IsType<JObject>(queueMessage.Event);
            // Use can use the inbuilt extension method to get the typed event dto instance
            var transferEventDto = queueMessage.GetDecodedEventDto<TransferEventDto>();
            Assert.Equal("0x296c61eaf5bea208bbabc65ae01c3bc5270fe386", transferEventDto.From);
            Assert.Equal("0x2a8f1a6af55b705b7daee0776d6f97302de2a839", transferEventDto.To);
            Assert.Equal(new BigInteger(119928660890733235), transferEventDto.Value);

        }

        /*
 * A sample message from the queue
 * the key is block number, transaction hash and log index
 * the message contains the raw log (FilterLog) and the decoded parameter values
 * it also contains extra state data that the processing chain can add or amend
 * the message can be retrieved from the queue and can be decoded into an event DTO (e.g. TransferEventDto)
{
"Key": "7540010_0x8d58abc578f5e321f2e6b7c0637ccc60fbf62b39b120691cbf19ff201f5069b0_132",
"State": {
    "EventAbiName": "Transfer",
    "EventSignature": "ddf252ad1be2c89b69c2b068fc378daa952ba7f163c4a11628f55a4df523b3ef",
    "TransactionHash": "0x8d58abc578f5e321f2e6b7c0637ccc60fbf62b39b120691cbf19ff201f5069b0",
    "LogIndex": 132,
    "HandlerInvocations": 1,
    "UtcNowMs": 1556800544598,
    "SubscriberId": 0,
    "EventSubscriptionId": 0
},
"Transaction": null,
"ParameterValues": [{
    "Order": 1,
    "Name": "_from",
    "AbiType": "address",
    "Value": "0x296c61eaf5bea208bbabc65ae01c3bc5270fe386",
    "Indexed": true
    }, {
    "Order": 2,
    "Name": "_to",
    "AbiType": "address",
    "Value": "0x2a8f1a6af55b705b7daee0776d6f97302de2a839",
    "Indexed": true
    }, {
    "Order": 3,
    "Name": "_value",
    "AbiType": "uint256",
    "Value": 119928660890733235,
    "Indexed": false
}
],
"Log": {
    "removed": false,
    "type": null,
    "logIndex": "0x84",
    "transactionHash": "0x8d58abc578f5e321f2e6b7c0637ccc60fbf62b39b120691cbf19ff201f5069b0",
    "transactionIndex": "0x95",
    "blockHash": "0x0ef49c4e3a11872084715808df7a07c4fa6f03dac0596b4e6c10b21b88f7d227",
    "blockNumber": "0x730d2a",
    "address": "0x9f8f72aa9304c8b593d555f12ef6589cc3a579a2",
    "data": "0x00000000000000000000000000000000000000000000000001aa127b4ec7cab3",
    "topics": ["0xddf252ad1be2c89b69c2b068fc378daa952ba7f163c4a11628f55a4df523b3ef", "0x000000000000000000000000296c61eaf5bea208bbabc65ae01c3bc5270fe386", "0x0000000000000000000000002a8f1a6af55b705b7daee0776d6f97302de2a839"]
}
}

 */

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
