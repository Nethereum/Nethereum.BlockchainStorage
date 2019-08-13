using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.BlockchainProcessing.Processor;
using Nethereum.BlockchainProcessing.Queue;
using Nethereum.BlockchainProcessing.Queue.Azure.Processing.Logs;
using Nethereum.Contracts;
using Nethereum.RPC.Eth.DTOs;
using System;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;
using Nethereum.Microsoft.Configuration.Utils;
using Xunit;

namespace Nethereum.BlockchainStorage.Samples.Queuing
{
    public class AzureQueueExamples
    {
        [Event("Transfer")]
        public class TransferEventDTO : IEventDTO
        {
            [Parameter("address", "_from", 1, true)]
            public string From { get; set; }

            [Parameter("address", "_to", 2, true)]
            public string To { get; set; }

            [Parameter("uint256", "_value", 3, false)]
            public BigInteger Value { get; set; }
        }

        [Event("Transfer")]
        public class Erc721TransferEvent
        {
            [Parameter("address", "_from", 1, true)]
            public string From { get; set; }

            [Parameter("address", "_to", 2, true)]
            public string To { get; set; }

            [Parameter("uint256", "_value", 3, true)]
            public BigInteger Value { get; set; }
        }

        [Event("Approval")]
        public class ApprovalEventDTO : IEventDTO
        {
            [Parameter("address", "_owner", 1, true)]
            public virtual string Owner { get; set; }
            [Parameter("address", "_spender", 2, true)]
            public virtual string Spender { get; set; }
            [Parameter("uint256", "_value", 3, false)]
            public virtual BigInteger Value { get; set; }
        }


        private readonly Web3.Web3 _web3;
        private readonly string _azureConnectionString;
        private const string URL = "https://rinkeby.infura.io/v3/7238211010344719ad14a89db874158c";

        public AzureQueueExamples()
        {
            _web3 = new Web3.Web3(URL);
            ConfigurationUtils.SetEnvironmentAsDevelopment();
            var config = ConfigurationUtils.Build(args: Array.Empty<string>(), userSecretsId: "Nethereum.BlockchainStore.AzureTables");
            _azureConnectionString = config["AzureStorageConnectionString"];
        }
        
        [Fact]
        public async Task AnyContractAnyLog()
        {
            const string QUEUE_NAME = "any-contract-any-log";
            var azureQueueFactory = new AzureStorageQueueFactory(_azureConnectionString);
            try 
            { 
                var queue = await azureQueueFactory.GetOrCreateQueueAsync(QUEUE_NAME);
                var eventLogProcessor = new EventLogQueueProcessor(queue);

                var logProcessor = _web3.Processing.Logs.CreateProcessor(logProcessor: eventLogProcessor);

                //if we need to stop the processor mid execution - call cancel on the token
                var cancellationTokenSource = new CancellationTokenSource();

                //crawl the required block range
                await logProcessor.ExecuteAsync(
                    toBlockNumber: 3146690,
                    cancellationToken: cancellationTokenSource.Token,
                    startAtBlockNumberIfNotProcessed: 3146684);

                Assert.Equal(65, await queue.GetApproxMessageCountAsync());
            }
            finally
            {
                await azureQueueFactory.DeleteQueueAsync(QUEUE_NAME);
            }
        }

        [Fact]
        public async Task AnyContractOneEvent()
        {
            const string QUEUE_NAME = "any-contract-one-event";
            var azureQueueFactory = new AzureStorageQueueFactory(_azureConnectionString);
            try
            {
                var queue = await azureQueueFactory.GetOrCreateQueueAsync(QUEUE_NAME);
                var eventLogProcessor = new EventLogQueueProcessor<TransferEventDTO>(queue);

                var logProcessor = _web3.Processing.Logs.CreateProcessor(logProcessor: eventLogProcessor);

                //if we need to stop the processor mid execution - call cancel on the token
                var cancellationTokenSource = new CancellationTokenSource();

                //crawl the required block range
                await logProcessor.ExecuteAsync(
                    toBlockNumber: 3146690,
                    cancellationToken: cancellationTokenSource.Token,
                    startAtBlockNumberIfNotProcessed: 3146684);

                Assert.Equal(13, await queue.GetApproxMessageCountAsync());
            }
            finally
            {
                await azureQueueFactory.DeleteQueueAsync(QUEUE_NAME);
            }
        }

        [Fact]
        public async Task AnyContractMultipleEvents()
        {
            const string ERC20_QUEUE_NAME = "any-contract-one-event-erc20";
            const string ERC721_QUEUE_NAME = "any-contract-one-event-erc721";

            var azureQueueFactory = new AzureStorageQueueFactory(_azureConnectionString);
            try
            {
                var erc20Queue = await azureQueueFactory.GetOrCreateQueueAsync(ERC20_QUEUE_NAME);
                var erc721Queue = await azureQueueFactory.GetOrCreateQueueAsync(ERC721_QUEUE_NAME);

                var erc20TransferProcessor = new EventLogQueueProcessor<TransferEventDTO>(erc20Queue);
                var erc721TransferProcessor = new EventLogQueueProcessor<Erc721TransferEvent>(erc721Queue);
                var logProcessors = new ProcessorHandler<FilterLog>[] {erc20TransferProcessor, erc721TransferProcessor};

                var logProcessor = _web3.Processing.Logs.CreateProcessor(logProcessors);

                //if we need to stop the processor mid execution - call cancel on the token
                var cancellationTokenSource = new CancellationTokenSource();

                //crawl the required block range
                await logProcessor.ExecuteAsync(
                    toBlockNumber: 3146690,
                    cancellationToken: cancellationTokenSource.Token,
                    startAtBlockNumberIfNotProcessed: 3146684);

                Assert.Equal(13, await erc20Queue.GetApproxMessageCountAsync());
                Assert.Equal(3, await erc721Queue.GetApproxMessageCountAsync());
            }
            finally
            {
                await azureQueueFactory.DeleteQueueAsync(ERC20_QUEUE_NAME);
                await azureQueueFactory.DeleteQueueAsync(ERC721_QUEUE_NAME);
            }
        }

        [Fact]
        public async Task OneContractAnyLog()
        {
            const string QUEUE_NAME = "one-contract-any-log";
            var azureQueueFactory = new AzureStorageQueueFactory(_azureConnectionString);
            try
            {
                var queue = await azureQueueFactory.GetOrCreateQueueAsync(QUEUE_NAME);
                var eventLogProcessor = new EventLogQueueProcessor(queue);

                var contractFilter = new NewFilterInput { Address = new[] { "0x109424946d5aa4425b2dc1934031d634cdad3f90" } };

                var logProcessor = _web3.Processing.Logs.CreateProcessor(logProcessor: eventLogProcessor, filter: contractFilter);

                //if we need to stop the processor mid execution - call cancel on the token
                var cancellationTokenSource = new CancellationTokenSource();

                //crawl the required block range
                await logProcessor.ExecuteAsync(
                    toBlockNumber: 3146690,
                    cancellationToken: cancellationTokenSource.Token,
                    startAtBlockNumberIfNotProcessed: 3146684);

                Assert.Equal(4, await queue.GetApproxMessageCountAsync());
            }
            finally
            {
                await azureQueueFactory.DeleteQueueAsync(QUEUE_NAME);
            }
        }

        [Fact]
        public async Task OneContractOneEvent()
        {
            const string QUEUE_NAME = "one-contract-one-event";
            var azureQueueFactory = new AzureStorageQueueFactory(_azureConnectionString);
            try
            {
                var queue = await azureQueueFactory.GetOrCreateQueueAsync(QUEUE_NAME);
                var eventLogProcessor = new EventLogQueueProcessor<TransferEventDTO>(queue);

                var contractFilter = new NewFilterInput { Address = new[] { "0x109424946d5aa4425b2dc1934031d634cdad3f90" } };

                var logProcessor = _web3.Processing.Logs.CreateProcessor(logProcessor: eventLogProcessor, filter: contractFilter);

                //if we need to stop the processor mid execution - call cancel on the token
                var cancellationTokenSource = new CancellationTokenSource();

                //crawl the required block range
                await logProcessor.ExecuteAsync(
                    toBlockNumber: 3146690,
                    cancellationToken: cancellationTokenSource.Token,
                    startAtBlockNumberIfNotProcessed: 3146684);

                Assert.Equal(1, await queue.GetApproxMessageCountAsync());
            }
            finally
            {
                await azureQueueFactory.DeleteQueueAsync(QUEUE_NAME);
            }
        }

        [Fact]
        public async Task OneContractMultipleEvents()
        {
            const string ERC20_QUEUE_NAME = "one-contract-multi-event-erc20";
            const string APPROVAL_QUEUE = "one-contract-multi-event-approval";

            var azureQueueFactory = new AzureStorageQueueFactory(_azureConnectionString);
            try
            {
                var erc20Queue = await azureQueueFactory.GetOrCreateQueueAsync(ERC20_QUEUE_NAME);
                var approvalQueue = await azureQueueFactory.GetOrCreateQueueAsync(APPROVAL_QUEUE);

                var erc20TransferProcessor = new EventLogQueueProcessor<TransferEventDTO>(erc20Queue);
                var approvalProcessor = new EventLogQueueProcessor<ApprovalEventDTO>(approvalQueue);
                var logProcessors = new ProcessorHandler<FilterLog>[] { erc20TransferProcessor, approvalProcessor };

                var contractFilter = new NewFilterInput
                {
                    Address = new[] { "0x9EDCb9A9c4d34b5d6A082c86cb4f117A1394F831" }
                };

                var logProcessor = _web3.Processing.Logs.CreateProcessor(logProcessors: logProcessors, filter: contractFilter);

                //if we need to stop the processor mid execution - call cancel on the token
                var cancellationTokenSource = new CancellationTokenSource();

                //crawl the required block range
                await logProcessor.ExecuteAsync(
                    toBlockNumber: 3621716,
                    cancellationToken: cancellationTokenSource.Token,
                    startAtBlockNumberIfNotProcessed: 3621715);

                Assert.Equal(2, await erc20Queue.GetApproxMessageCountAsync());
                Assert.Equal(1, await approvalQueue.GetApproxMessageCountAsync());
            }
            finally
            {
                await azureQueueFactory.DeleteQueueAsync(ERC20_QUEUE_NAME);
                await azureQueueFactory.DeleteQueueAsync(APPROVAL_QUEUE);
            }
        }

        [Fact]
        public async Task ManyContractsAnyLog()
        {
            const string QUEUE_NAME = "many-contract-any-log";
            var azureQueueFactory = new AzureStorageQueueFactory(_azureConnectionString);
            try
            {
                var queue = await azureQueueFactory.GetOrCreateQueueAsync(QUEUE_NAME);
                var eventLogProcessor = new EventLogQueueProcessor(queue);

                var contractAddresses = new[] { "0x109424946d5aa4425b2dc1934031d634cdad3f90", "0x16c45b25c4817bdedfce770f795790795c9505a6" };

                var contractFilter = new NewFilterInput { Address = contractAddresses };

                var logProcessor = _web3.Processing.Logs.CreateProcessor(logProcessor: eventLogProcessor, filter: contractFilter);

                //if we need to stop the processor mid execution - call cancel on the token
                var cancellationTokenSource = new CancellationTokenSource();

                //crawl the required block range
                await logProcessor.ExecuteAsync(
                    toBlockNumber: 3146690,
                    cancellationToken: cancellationTokenSource.Token,
                    startAtBlockNumberIfNotProcessed: 3146684);

                Assert.Equal(8, await queue.GetApproxMessageCountAsync());
            }
            finally
            {
                await azureQueueFactory.DeleteQueueAsync(QUEUE_NAME);
            }
        }

        [Fact]
        public async Task ManyContractsOneEvent()
        {
            const string QUEUE_NAME = "many-contracts-one-event";
            var azureQueueFactory = new AzureStorageQueueFactory(_azureConnectionString);
            try
            {
                var queue = await azureQueueFactory.GetOrCreateQueueAsync(QUEUE_NAME);
                var eventLogProcessor = new EventLogQueueProcessor<TransferEventDTO>(queue);

                var contractAddresses = new[] { "0x109424946d5aa4425b2dc1934031d634cdad3f90", "0x16c45b25c4817bdedfce770f795790795c9505a6" };

                var contractFilter = new NewFilterInput { Address = contractAddresses };

                var logProcessor = _web3.Processing.Logs.CreateProcessor(logProcessor: eventLogProcessor, filter: contractFilter);

                //if we need to stop the processor mid execution - call cancel on the token
                var cancellationTokenSource = new CancellationTokenSource();

                //crawl the required block range
                await logProcessor.ExecuteAsync(
                    toBlockNumber: 3146690,
                    cancellationToken: cancellationTokenSource.Token,
                    startAtBlockNumberIfNotProcessed: 3146684);

                Assert.Equal(5, await queue.GetApproxMessageCountAsync());
            }
            finally
            {
                await azureQueueFactory.DeleteQueueAsync(QUEUE_NAME);
            }
        }

        [Fact]
        public async Task ManyContractsMultipleEvents()
        {
            const string ERC20_QUEUE_NAME = "many-contract-multi-event-erc20";
            const string APPROVAL_QUEUE_NAME = "many-contract-multi-event-approval";

            var azureQueueFactory = new AzureStorageQueueFactory(_azureConnectionString);
            try
            {
                var erc20Queue = await azureQueueFactory.GetOrCreateQueueAsync(ERC20_QUEUE_NAME);
                var approvalQueue = await azureQueueFactory.GetOrCreateQueueAsync(APPROVAL_QUEUE_NAME);

                var erc20TransferProcessor = new EventLogQueueProcessor<TransferEventDTO>(erc20Queue);
                var approvalProcessor = new EventLogQueueProcessor<ApprovalEventDTO>(approvalQueue);
                var logProcessors = new ProcessorHandler<FilterLog>[] { erc20TransferProcessor, approvalProcessor };

                var contractAddresses = new[] { "0x9EDCb9A9c4d34b5d6A082c86cb4f117A1394F831", "0xafbfefa496ae205cf4e002dee11517e6d6da3ef6" };

                var contractFilter = new NewFilterInput { Address = contractAddresses };

                var logProcessor = _web3.Processing.Logs.CreateProcessor(logProcessors, filter: contractFilter);

                //if we need to stop the processor mid execution - call cancel on the token
                var cancellationTokenSource = new CancellationTokenSource();

                //crawl the required block range
                await logProcessor.ExecuteAsync(
                    toBlockNumber: 3621716,
                    cancellationToken: cancellationTokenSource.Token,
                    startAtBlockNumberIfNotProcessed: 3621715);

                Assert.Equal(2, await erc20Queue.GetApproxMessageCountAsync());
                Assert.Equal(1, await approvalQueue.GetApproxMessageCountAsync());
            }
            finally
            {
                await azureQueueFactory.DeleteQueueAsync(ERC20_QUEUE_NAME);
                await azureQueueFactory.DeleteQueueAsync(APPROVAL_QUEUE_NAME);
            }
        }

        public class MessageToQueue
        {
            public MessageToQueue(FilterLog log)
            {
                BlockNumber = log.BlockNumber.Value.ToString();
                TransactionHash = log.TransactionHash;
                LogIndex = log.LogIndex.ToString();
            }

            public MessageToQueue(EventLog<TransferEventDTO> transferEventLog)
            {
                BlockNumber = transferEventLog.Log.BlockNumber.Value.ToString();
                TransactionHash = transferEventLog.Log.TransactionHash;
                LogIndex = transferEventLog.Log.LogIndex.ToString();
                EventName = transferEventLog.Event.GetEventABI().Name;
            }

            public string EventName { get; set;}

            public string BlockNumber { get;set;}
            public string TransactionHash { get;set;}
            public string LogIndex { get;set;}
        }

        [Fact]
        public async Task MappingALogToACustomQueueMessage()
        {
            const string QUEUE_NAME = "mapping-from-log";
            var azureQueueFactory = new AzureStorageQueueFactory(_azureConnectionString);
            try
            {
                var queue = await azureQueueFactory.GetOrCreateQueueAsync(QUEUE_NAME);

                var eventLogProcessor = new EventLogQueueProcessor(
                    destinationQueue: queue, mapper: (log) => new MessageToQueue(log));

                var logProcessor = _web3.Processing.Logs.CreateProcessor(logProcessor: eventLogProcessor);

                //if we need to stop the processor mid execution - call cancel on the token
                var cancellationTokenSource = new CancellationTokenSource();

                //crawl the required block range
                await logProcessor.ExecuteAsync(
                    toBlockNumber: 3146690,
                    cancellationToken: cancellationTokenSource.Token,
                    startAtBlockNumberIfNotProcessed: 3146684);

                Assert.Equal(65, await queue.GetApproxMessageCountAsync());
            }
            finally
            {
                await azureQueueFactory.DeleteQueueAsync(QUEUE_NAME);
            }
        }

        [Fact]
        public async Task MappingAnEventLogToACustomQueueMessage()
        {
            const string QUEUE_NAME = "mapping-from-event-log";
            var azureQueueFactory = new AzureStorageQueueFactory(_azureConnectionString);
            try
            {
                var queue = await azureQueueFactory.GetOrCreateQueueAsync(QUEUE_NAME);

                var eventLogProcessor = new EventLogQueueProcessor<TransferEventDTO>(
                    destinationQueue: queue, mapper: (log) => new MessageToQueue(log));

                var logProcessor = _web3.Processing.Logs.CreateProcessor(logProcessor: eventLogProcessor);

                //if we need to stop the processor mid execution - call cancel on the token
                var cancellationTokenSource = new CancellationTokenSource();

                //crawl the required block range
                await logProcessor.ExecuteAsync(
                    toBlockNumber: 3146690,
                    cancellationToken: cancellationTokenSource.Token,
                    startAtBlockNumberIfNotProcessed: 3146684);

                Assert.Equal(13, await queue.GetApproxMessageCountAsync());
            }
            finally
            {
                await azureQueueFactory.DeleteQueueAsync(QUEUE_NAME);
            }
        }

        [Fact]
        public async Task EventSpecificCriteria()
        {
            const string QUEUE_NAME = "with-event-specific-criteria";
            var azureQueueFactory = new AzureStorageQueueFactory(_azureConnectionString);
            try
            {
                var queue = await azureQueueFactory.GetOrCreateQueueAsync(QUEUE_NAME);

                var minValue = BigInteger.Parse("5000000000000000000");

                var hits = 0;
                var criteria = new Func<EventLog<TransferEventDTO>, bool>(transferLog => 
                {
                    var match = transferLog.Event.Value >= minValue;
                    if(match) hits++;
                    return match;
                });

                var eventLogProcessor = new EventLogQueueProcessor<TransferEventDTO>(
                    destinationQueue: queue, 
                    eventCriteria:  criteria);

                var logProcessor = _web3.Processing.Logs.CreateProcessor(logProcessor: eventLogProcessor);

                //if we need to stop the processor mid execution - call cancel on the token
                var cancellationTokenSource = new CancellationTokenSource();

                //crawl the required block range
                await logProcessor.ExecuteAsync(
                    toBlockNumber: 3146690,
                    cancellationToken: cancellationTokenSource.Token,
                    startAtBlockNumberIfNotProcessed: 3146684);

                await Task.Delay(2000); //give time for queue to update

                Assert.Equal(6, await queue.GetApproxMessageCountAsync());
            }
            finally
            {
                await azureQueueFactory.DeleteQueueAsync(QUEUE_NAME);
            }
        }

        [Fact]
        public async Task FilterLogCriteria()
        {
            const string QUEUE_NAME = "with-filter-log-criteria";
            var azureQueueFactory = new AzureStorageQueueFactory(_azureConnectionString);
            try
            {
                var queue = await azureQueueFactory.GetOrCreateQueueAsync(QUEUE_NAME);

                var eventLogProcessor = new EventLogQueueProcessor(
                    destinationQueue: queue,
                    criteria: (filterLog) => filterLog.Removed == false);

                var logProcessor = _web3.Processing.Logs.CreateProcessor(logProcessor: eventLogProcessor);

                //if we need to stop the processor mid execution - call cancel on the token
                var cancellationTokenSource = new CancellationTokenSource();

                //crawl the required block range
                await logProcessor.ExecuteAsync(
                    toBlockNumber: 3146690,
                    cancellationToken: cancellationTokenSource.Token,
                    startAtBlockNumberIfNotProcessed: 3146684);

                Assert.Equal(65, await queue.GetApproxMessageCountAsync());
            }
            finally
            {
                await azureQueueFactory.DeleteQueueAsync(QUEUE_NAME);
            }
        }



    }
}
