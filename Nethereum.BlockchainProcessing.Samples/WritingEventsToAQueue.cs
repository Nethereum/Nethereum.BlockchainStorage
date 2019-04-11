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
        public static int PARTITION = 3;
        const ulong MIN_BLOCK_NUMBER = 7540000;
        const uint MAX_BLOCKS_PER_BATCH = 100;

        /// <summary>
        /// Grab all events for the MakerDAO contract and write to an Azure queue
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task QueueAllEventsForMakerDAOContract()
        {
            var config = TestConfiguration.LoadConfig();
            string azureStorageConnectionString = config["AzureStorageConnectionString"];

            var blockchainProxy = new BlockchainProxyService(TestConfiguration.BlockchainUrls.Infura.Mainnet);

            // queue components
            var queueFactory = new AzureSubscriberQueueFactory(azureStorageConnectionString);
            var queue = await queueFactory.GetOrCreateQueueAsync("makerdaoevents");

            //event subscription set up
            var contractAbi = new ABIDeserialiser().DeserialiseContract(MAKER_DAO_ABI);
            var eventSubscription = new EventSubscription(contractAbi.Events, new[] { MAKER_CONTRACT_ADDRESS });
            eventSubscription.AddQueueHandler(queue);

            // progress repo (dictates which block ranges to process next)
            // maintain separate progress per partition via a prefix
            var storageCloudSetup = new CloudTableSetup(azureStorageConnectionString, prefix: $"Partition{PARTITION}");
            var blockProgressRepo = storageCloudSetup.CreateBlockProgressRepository();

            //this ensures we only query the chain for events relating to this contract
            var makerAddressFilter = new NewFilterInput() { Address = new[] { MAKER_CONTRACT_ADDRESS } };

            // load service
            var progressService = new BlockProgressService(blockchainProxy, MIN_BLOCK_NUMBER, blockProgressRepo);
            var logProcessor = new BlockchainLogProcessor(blockchainProxy, new[] { eventSubscription }, makerAddressFilter);
            var batchProcessorService = new BlockchainBatchProcessorService(logProcessor, progressService, MAX_BLOCKS_PER_BATCH);

            // execute
            try
            {
                var ctx = new System.Threading.CancellationTokenSource();
                var rangeProcessed = await batchProcessorService.ProcessLatestBlocksAsync(ctx.Token);

                Assert.Equal(16, eventSubscription.State.GetInt("EventsHandled"));
                Assert.Equal(16, await queue.GetApproxMessageCountAsync());
            }
            finally
            {
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
