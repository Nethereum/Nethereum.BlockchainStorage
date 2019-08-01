using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.BlockchainProcessing.Processor;
using Nethereum.BlockchainProcessing.Queue;
using Nethereum.BlockchainProcessing.Queue.Azure.Processing.Logs;
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
        public Task OneContractAnyLog()
        {
            throw new NotImplementedException();
        }

        [Fact]
        public Task OneContractOneEvent()
        {
            throw new NotImplementedException();
        }

        [Fact]
        public Task OneContractMultipleEvents()
        {
            throw new NotImplementedException();
        }

        [Fact]
        public Task ManyContractsAnyLog()
        {
            throw new NotImplementedException();
        }

        [Fact]
        public Task ManyContractsOneEvent()
        {
            throw new NotImplementedException();
        }

        [Fact]
        public Task ManyContractsMultipleEvents()
        {
            throw new NotImplementedException();
        }

        public class MessageToQueue
        {
            public MessageToQueue(FilterLog log)
            {
                BlockNumber = log.BlockNumber.Value.ToString();
                TransactionHash = log.TransactionHash;
                LogIndex = log.LogIndex.ToString();
            }

            public string BlockNumber { get;set;}
            public string TransactionHash { get;set;}
            public string LogIndex { get;set;}
        }

        [Fact]
        public async Task WithMapping()
        {
            const string QUEUE_NAME = "with-mapping";
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
        public async Task EventSpecificCriteria()
        {
            const string QUEUE_NAME = "with-event-specific-criteria";
            var azureQueueFactory = new AzureStorageQueueFactory(_azureConnectionString);
            try
            {
                var queue = await azureQueueFactory.GetOrCreateQueueAsync(QUEUE_NAME);

                var minValue = BigInteger.Parse("5000000000000000000");

                var eventLogProcessor = new EventLogQueueProcessor<TransferEventDTO>(
                    destinationQueue: queue, 
                    eventCriteria:  (transferLog) => transferLog.Event.Value >= minValue);

                var logProcessor = _web3.Processing.Logs.CreateProcessor(logProcessor: eventLogProcessor);

                //if we need to stop the processor mid execution - call cancel on the token
                var cancellationTokenSource = new CancellationTokenSource();

                //crawl the required block range
                await logProcessor.ExecuteAsync(
                    toBlockNumber: 3146690,
                    cancellationToken: cancellationTokenSource.Token,
                    startAtBlockNumberIfNotProcessed: 3146684);

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
