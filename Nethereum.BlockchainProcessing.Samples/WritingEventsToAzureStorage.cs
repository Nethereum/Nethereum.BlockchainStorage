using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.ABI.JsonDeserialisation;
using Nethereum.BlockchainProcessing.BlockchainProxy;
using Nethereum.BlockchainProcessing.Processing;
using Nethereum.BlockchainProcessing.Processing.Logs;
using Nethereum.BlockchainProcessing.Processing.Logs.Handling;
using Nethereum.BlockchainProcessing.Processing.Logs.Handling.Handlers;
using Nethereum.BlockchainStore.AzureTables.Bootstrap;
using Nethereum.BlockchainStore.AzureTables.Entities;
using Nethereum.BlockchainStore.AzureTables.Factories;
using Nethereum.BlockchainStore.AzureTables.Repositories;
using Nethereum.BlockchainStore.Repositories.Handlers;
using Nethereum.RPC.Eth.DTOs;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using Xunit;

namespace Nethereum.BlockchainProcessing.Samples
{
    public class WritingEventsToAzureStorage
    {

        /// <summary>
        /// Represents a typical ERC20 Transfer Event
        /// </summary>
        [Event("Transfer")]
        public class TransferEventDto : IEventDTO
        {
            [Parameter("address", "_from", 1, true)]
            public string From { get; set; }

            [Parameter("address", "_to", 2, true)]
            public string To { get; set; }

            [Parameter("uint256", "_value", 3, false)]
            public BigInteger Value { get; set; }
        }

        private static string MAKER_DAO_ABI = "[{\"constant\":true,\"inputs\":[],\"name\":\"name\",\"outputs\":[{\"name\":\"\",\"type\":\"bytes32\"}],\"payable\":false,\"stateMutability\":\"view\",\"type\":\"function\"},{\"constant\":false,\"inputs\":[],\"name\":\"stop\",\"outputs\":[],\"payable\":false,\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"constant\":false,\"inputs\":[{\"name\":\"guy\",\"type\":\"address\"},{\"name\":\"wad\",\"type\":\"uint256\"}],\"name\":\"approve\",\"outputs\":[{\"name\":\"\",\"type\":\"bool\"}],\"payable\":false,\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"constant\":false,\"inputs\":[{\"name\":\"owner_\",\"type\":\"address\"}],\"name\":\"setOwner\",\"outputs\":[],\"payable\":false,\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"constant\":true,\"inputs\":[],\"name\":\"totalSupply\",\"outputs\":[{\"name\":\"\",\"type\":\"uint256\"}],\"payable\":false,\"stateMutability\":\"view\",\"type\":\"function\"},{\"constant\":false,\"inputs\":[{\"name\":\"src\",\"type\":\"address\"},{\"name\":\"dst\",\"type\":\"address\"},{\"name\":\"wad\",\"type\":\"uint256\"}],\"name\":\"transferFrom\",\"outputs\":[{\"name\":\"\",\"type\":\"bool\"}],\"payable\":false,\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"constant\":true,\"inputs\":[],\"name\":\"decimals\",\"outputs\":[{\"name\":\"\",\"type\":\"uint256\"}],\"payable\":false,\"stateMutability\":\"view\",\"type\":\"function\"},{\"constant\":false,\"inputs\":[{\"name\":\"guy\",\"type\":\"address\"},{\"name\":\"wad\",\"type\":\"uint256\"}],\"name\":\"mint\",\"outputs\":[],\"payable\":false,\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"constant\":false,\"inputs\":[{\"name\":\"wad\",\"type\":\"uint256\"}],\"name\":\"burn\",\"outputs\":[],\"payable\":false,\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"constant\":false,\"inputs\":[{\"name\":\"name_\",\"type\":\"bytes32\"}],\"name\":\"setName\",\"outputs\":[],\"payable\":false,\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"constant\":true,\"inputs\":[{\"name\":\"src\",\"type\":\"address\"}],\"name\":\"balanceOf\",\"outputs\":[{\"name\":\"\",\"type\":\"uint256\"}],\"payable\":false,\"stateMutability\":\"view\",\"type\":\"function\"},{\"constant\":true,\"inputs\":[],\"name\":\"stopped\",\"outputs\":[{\"name\":\"\",\"type\":\"bool\"}],\"payable\":false,\"stateMutability\":\"view\",\"type\":\"function\"},{\"constant\":false,\"inputs\":[{\"name\":\"authority_\",\"type\":\"address\"}],\"name\":\"setAuthority\",\"outputs\":[],\"payable\":false,\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"constant\":true,\"inputs\":[],\"name\":\"owner\",\"outputs\":[{\"name\":\"\",\"type\":\"address\"}],\"payable\":false,\"stateMutability\":\"view\",\"type\":\"function\"},{\"constant\":true,\"inputs\":[],\"name\":\"symbol\",\"outputs\":[{\"name\":\"\",\"type\":\"bytes32\"}],\"payable\":false,\"stateMutability\":\"view\",\"type\":\"function\"},{\"constant\":false,\"inputs\":[{\"name\":\"guy\",\"type\":\"address\"},{\"name\":\"wad\",\"type\":\"uint256\"}],\"name\":\"burn\",\"outputs\":[],\"payable\":false,\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"constant\":false,\"inputs\":[{\"name\":\"wad\",\"type\":\"uint256\"}],\"name\":\"mint\",\"outputs\":[],\"payable\":false,\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"constant\":false,\"inputs\":[{\"name\":\"dst\",\"type\":\"address\"},{\"name\":\"wad\",\"type\":\"uint256\"}],\"name\":\"transfer\",\"outputs\":[{\"name\":\"\",\"type\":\"bool\"}],\"payable\":false,\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"constant\":false,\"inputs\":[{\"name\":\"dst\",\"type\":\"address\"},{\"name\":\"wad\",\"type\":\"uint256\"}],\"name\":\"push\",\"outputs\":[],\"payable\":false,\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"constant\":false,\"inputs\":[{\"name\":\"src\",\"type\":\"address\"},{\"name\":\"dst\",\"type\":\"address\"},{\"name\":\"wad\",\"type\":\"uint256\"}],\"name\":\"move\",\"outputs\":[],\"payable\":false,\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"constant\":false,\"inputs\":[],\"name\":\"start\",\"outputs\":[],\"payable\":false,\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"constant\":true,\"inputs\":[],\"name\":\"authority\",\"outputs\":[{\"name\":\"\",\"type\":\"address\"}],\"payable\":false,\"stateMutability\":\"view\",\"type\":\"function\"},{\"constant\":false,\"inputs\":[{\"name\":\"guy\",\"type\":\"address\"}],\"name\":\"approve\",\"outputs\":[{\"name\":\"\",\"type\":\"bool\"}],\"payable\":false,\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"constant\":true,\"inputs\":[{\"name\":\"src\",\"type\":\"address\"},{\"name\":\"guy\",\"type\":\"address\"}],\"name\":\"allowance\",\"outputs\":[{\"name\":\"\",\"type\":\"uint256\"}],\"payable\":false,\"stateMutability\":\"view\",\"type\":\"function\"},{\"constant\":false,\"inputs\":[{\"name\":\"src\",\"type\":\"address\"},{\"name\":\"wad\",\"type\":\"uint256\"}],\"name\":\"pull\",\"outputs\":[],\"payable\":false,\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"name\":\"symbol_\",\"type\":\"bytes32\"}],\"payable\":false,\"stateMutability\":\"nonpayable\",\"type\":\"constructor\"},{\"anonymous\":false,\"inputs\":[{\"indexed\":true,\"name\":\"guy\",\"type\":\"address\"},{\"indexed\":false,\"name\":\"wad\",\"type\":\"uint256\"}],\"name\":\"Mint\",\"type\":\"event\"},{\"anonymous\":false,\"inputs\":[{\"indexed\":true,\"name\":\"guy\",\"type\":\"address\"},{\"indexed\":false,\"name\":\"wad\",\"type\":\"uint256\"}],\"name\":\"Burn\",\"type\":\"event\"},{\"anonymous\":false,\"inputs\":[{\"indexed\":true,\"name\":\"authority\",\"type\":\"address\"}],\"name\":\"LogSetAuthority\",\"type\":\"event\"},{\"anonymous\":false,\"inputs\":[{\"indexed\":true,\"name\":\"owner\",\"type\":\"address\"}],\"name\":\"LogSetOwner\",\"type\":\"event\"},{\"anonymous\":true,\"inputs\":[{\"indexed\":true,\"name\":\"sig\",\"type\":\"bytes4\"},{\"indexed\":true,\"name\":\"guy\",\"type\":\"address\"},{\"indexed\":true,\"name\":\"foo\",\"type\":\"bytes32\"},{\"indexed\":true,\"name\":\"bar\",\"type\":\"bytes32\"},{\"indexed\":false,\"name\":\"wad\",\"type\":\"uint256\"},{\"indexed\":false,\"name\":\"fax\",\"type\":\"bytes\"}],\"name\":\"LogNote\",\"type\":\"event\"},{\"anonymous\":false,\"inputs\":[{\"indexed\":true,\"name\":\"from\",\"type\":\"address\"},{\"indexed\":true,\"name\":\"to\",\"type\":\"address\"},{\"indexed\":false,\"name\":\"value\",\"type\":\"uint256\"}],\"name\":\"Transfer\",\"type\":\"event\"},{\"anonymous\":false,\"inputs\":[{\"indexed\":true,\"name\":\"owner\",\"type\":\"address\"},{\"indexed\":true,\"name\":\"spender\",\"type\":\"address\"},{\"indexed\":false,\"name\":\"value\",\"type\":\"uint256\"}],\"name\":\"Approval\",\"type\":\"event\"}]";
        public static string MAKER_CONTRACT_ADDRESS = "0x9f8F72aA9304c8B593d555F12eF6589cC3A579A2";
        const ulong MIN_BLOCK_NUMBER = 7540000;
        const uint MAX_BLOCKS_PER_BATCH = 100;

        [Fact]
        public async Task WriteTransferEventsForMakerDAOToAzureStorage()
        {
            // Load config
            //  - this will contain the secrets and connection strings we don't want to hard code
            var config = TestConfiguration.LoadConfig();
            string azureStorageConnectionString = config["AzureStorageConnectionString"];

            // Create a proxy for the blockchain
            var web3 = new Web3.Web3(TestConfiguration.BlockchainUrls.Infura.Mainnet);

            // Create an Azure Table Storage Factory 
            //  - The factory communicates with Azure to create and get different tables
            var tableStorageFactory = new AzureTablesSubscriberRepositoryFactory(azureStorageConnectionString);

            // Create a Handler for a Table
            // - It wraps a table repository 
            // - This is where we're going to put the matching event logs
            // - we're supplying a table prefix 
            // - the actual table name would be "<prefix>TransactionLogs")
            // - this allows us to have different tables for different types of event logs
            // - the handler implements ILogHandler
            // - ILogHandler is a really simple interface to implement if you wish to customise the storage 
            var storageHandlerForLogs = await tableStorageFactory.GetLogRepositoryHandlerAsync(tablePrefix: "makerdaotransfersstorage");

            // Create an event subscription specifically for ERC20 Transfers
            // - Passing in the maker dao address to ensure only logs with a matching address are processed
            // - There is an option to pass an implementation of IEventHandlerHistoryRepository in to the constructor
            // - This would record history for each event handler and is used to prevent duplication
            var eventSubscription = new EventSubscription<TransferEventDto>(
                contractAddressesToMatch: new[] { MAKER_CONTRACT_ADDRESS });

            // Assign the storage handler to the event subscription
            // - Matching events will be passed to the handler 
            // - internally the handler passes the events to the repository layer which writes them to Azure
            eventSubscription.AddStorageHandler(storageHandlerForLogs);

            // Azure storage setup
            // - this example reads and writes block progress to an Azure storage table
            // - to avoid collision with other samples we provide a prefix
            var storageCloudSetup = new CloudTableSetup(azureStorageConnectionString, prefix: $"makerdaotransfersstorage");

            // Create a progress repository
            //  - It stores and retrieves the most recent block processed
            var blockProgressRepo = storageCloudSetup.CreateBlockProgressRepository();

            // Create a progress service
            // - This uses the progress repo to dictate what blocks to process next
            // - The MIN_BLOCK_NUMBER dictates the starting point if the progress repo is empty or has fallen too far behind 
            var progressService = new BlockProgressService(web3, MIN_BLOCK_NUMBER, blockProgressRepo);

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
            var logProcessor = new BlockchainLogProcessor(web3, new[] { eventSubscription }, makerAddressFilter);

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

                // get the row count from azure storage
                // the querying on storage is limited 
                // the TransactionHash is the partitionkey and the rowkey is the LogIndex
                // this allows us to query by tx hash

                var logRepositoryHandler = storageHandlerForLogs as TransactionLogRepositoryHandler;
                var repository = logRepositoryHandler.TransactionLogRepository as TransactionLogRepository;
                
                var expectedTransactionHashes = new[]
                {
"0x8d58abc578f5e321f2e6b7c0637ccc60fbf62b39b120691cbf19ff201f5069b0",
"0x0bee561ac6bafb59bcc4c48fc4c1225aaedbab3e8089acea420140aafa47f3e5",
"0x6fc82b076fa7088581a80869cb9c7a08d7f8e897670a9f67e39139b39246da7e",
"0xdc2ee28db35ed5dbbc9e18a7d6bdbacb6e6633a9fce1ecda99ea7e1cf4bc8c72",
"0xcd2fea48c84468f70c9a44c4ffd7b26064a2add8b72937edf593634d2501c1f6",
"0x3acf887420887148222aab1d25d4d4893794e505ef276cc4cb6a48fffc6cb381",
"0x96129f905589b2a95c26276aa7e8708a12381ddec50485d6684c4abf9a5a1d00"
                };

                List<TransactionLog> logsFromRepo = new List<TransactionLog>(); 
                foreach(var txHash in expectedTransactionHashes)
                {
                    logsFromRepo.AddRange(await repository.GetManyAsync(txHash));
                }

                Assert.Equal(11, logsFromRepo.Count);

            }
            finally
            {
                // delete any data from Azure
                await storageCloudSetup.GetCountersTable().DeleteIfExistsAsync();
                await tableStorageFactory.DeleteTablesAsync();
            }

        }


        [Fact]
        public async Task WriteAllEventsForMakerDAOToAzureStorage()
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
            var web3 = new Web3.Web3(TestConfiguration.BlockchainUrls.Infura.Mainnet);

            //  Get the maker DAO contract abi
            //  - from this we're able to match and decode the events in the contract
            var contractAbi = new ABIDeserialiser().DeserialiseContract(MAKER_DAO_ABI);

            // Create an event subscription for these events
            // - Passing in the maker dao address to ensure only logs with a matching address are processed
            // - There is an option to pass an implementation of IEventHandlerHistoryRepository in to the constructor
            // - This would record history for each event handler and is used to prevent duplication
            var eventSubscription = new EventSubscription(contractAbi.Events, new[] { MAKER_CONTRACT_ADDRESS });

            // Create an Azure Table Storage Factory 
            //  - The factory communicates with Azure to create and get different tables
            var tableStorageFactory = new AzureTablesSubscriberRepositoryFactory(azureStorageConnectionString);

            // Create a Handler for a Table
            // - It wraps a table repository (routing ILogHandler calls to ITransactionLogRepository)
            // - This is where we're going to put the matching event logs
            // - we're supplying a table prefix 
            // - the actual table name would be "<prefix>TransactionLogs"
            // - this allows us to have different tables for different groups of event logs
            // - the handler implements ILogHandler
            // - ILogHandler is a really simple interface to implement if you wish to customise the storage 
            var storageHandlerForLogs = await tableStorageFactory.GetLogRepositoryHandlerAsync(tablePrefix: "makerdaoalleventstorage");

            // Assign the storage handler to the event subscription
            // - Matching events will be passed to the handler 
            // - internally the handler passes the events to the repository layer which writes them to Azure
            eventSubscription.AddStorageHandler(storageHandlerForLogs);

            // Azure storage setup (for storing Block Progress)
            // - this example reads and writes block progress to an Azure storage table
            // - to avoid collision with other samples we provide a prefix
            var storageCloudSetup = new CloudTableSetup(azureStorageConnectionString, prefix: $"makerdaoalleventstorage");

            // Create a progress repository
            //  - It stores and retrieves the most recent block processed
            var blockProgressRepo = storageCloudSetup.CreateBlockProgressRepository();

            // Create a progress service
            // - This uses the progress repo to dictate what blocks to process next
            // - The MIN_BLOCK_NUMBER dictates the starting point if the progress repo is empty or has fallen too far behind 
            var progressService = new BlockProgressService(web3, MIN_BLOCK_NUMBER, blockProgressRepo);

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
            var logProcessor = new BlockchainLogProcessor(web3, new[] { eventSubscription }, makerAddressFilter);

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

                // get the row count from azure storage
                // querying azure table storage is limited/basic 
                // the TransactionHash is the partitionkey and the rowkey is the LogIndex
                // this allows us to query by tx hash

                var logRepositoryHandler = storageHandlerForLogs as TransactionLogRepositoryHandler;
                var repository = logRepositoryHandler.TransactionLogRepository as TransactionLogRepository;

                var expectedTransactionHashes = new[]
                {
"0x19db52e6f823b39aa1763321514a3f4a07c66140b90d99bc28b7f4bc318e0c9c",
"0x8d58abc578f5e321f2e6b7c0637ccc60fbf62b39b120691cbf19ff201f5069b0",
"0x2a7da4088595861792bcd6c4201bcacc8e93e4b931a9254da82594cd42ca9993",
"0x0bee561ac6bafb59bcc4c48fc4c1225aaedbab3e8089acea420140aafa47f3e5",
"0x6fc82b076fa7088581a80869cb9c7a08d7f8e897670a9f67e39139b39246da7e",
"0xdc2ee28db35ed5dbbc9e18a7d6bdbacb6e6633a9fce1ecda99ea7e1cf4bc8c72",
"0xcd2fea48c84468f70c9a44c4ffd7b26064a2add8b72937edf593634d2501c1f6",
"0x3acf887420887148222aab1d25d4d4893794e505ef276cc4cb6a48fffc6cb381",
"0x96129f905589b2a95c26276aa7e8708a12381ddec50485d6684c4abf9a5a1d00"
                };

                List<TransactionLog> logsFromRepo = new List<TransactionLog>();
                foreach (var txHash in expectedTransactionHashes)
                {
                    logsFromRepo.AddRange(await repository.GetManyAsync(txHash));
                }

                Assert.Equal(16, logsFromRepo.Count);

            }
            finally
            {
                // delete any data from Azure
                await storageCloudSetup.GetCountersTable().DeleteIfExistsAsync();
                await tableStorageFactory.DeleteTablesAsync();
            }

        }

    }
}
