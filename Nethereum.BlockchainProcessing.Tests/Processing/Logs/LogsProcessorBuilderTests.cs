using Moq;
using Nethereum.BlockchainProcessing.BlockchainProxy;
using Nethereum.BlockchainProcessing.Processing;
using Nethereum.BlockchainProcessing.Processing.Logs;
using Nethereum.BlockchainProcessing.Processing.Logs.Handling;
using Nethereum.Contracts;
using Nethereum.Hex.HexConvertors.Extensions;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.Web3;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Nethereum.BlockchainProcessing.Tests.Processing.Logs
{
    public class LogsProcessorBuilderTests
    {
        const string BLOCKCHAIN_URL = "http://localhost:8545/";

        [Fact]
        public void EventSpecific_Construction_FromUrl_CreatesBlockchainProxyService()
        {
            var processor = new LogsProcessorBuilder<TestData.Contracts.StandardContract.TransferEvent>(blockchainUrl: BLOCKCHAIN_URL);
            Assert.NotNull(processor.BlockchainProxyService);
        }

        [Fact]
        public void EventSpecific_Construction_AddsEventSpecificFilter()
        {
            var processor = new LogsProcessorBuilder<TestData.Contracts.StandardContract.TransferEvent>(blockchainUrl: BLOCKCHAIN_URL);
            Assert.Single(processor.Filters);
            Assert.Equal(TestData.Contracts.StandardContract.TransferEventAbi.Sha3Signature.EnsureHexPrefix(), processor.Filters.First().Topics[0]);
        }

        [Fact]
        public void EventSpecific_Construction_AddsAFilterForContractAddress()
        {
            const string CONTRACT_ADDRESS = "0x243e72b69141f6af525a9a5fd939668ee9f2b354";
            var processor = new LogsProcessorBuilder<TestData.Contracts.StandardContract.TransferEvent>(blockchainUrl: BLOCKCHAIN_URL, contractAddress: CONTRACT_ADDRESS);
            Assert.Single(processor.Filters);
            Assert.Equal(CONTRACT_ADDRESS, processor.Filters[0].Address[0]);
            Assert.Equal(TestData.Contracts.StandardContract.TransferEventAbi.Sha3Signature.EnsureHexPrefix(), processor.Filters[0].Topics[0]);
        }

        [Fact]
        public void EventSpecific_Construction_AddsAFilterForContractAddresses()
        {
            string[] CONTRACT_ADDRESSES = new[] { "0x243e72b69141f6af525a9a5fd939668ee9f2b354", "0x343e72b69141f6af525a9a5fd939668ee9f2b354" };
            var processor = new LogsProcessorBuilder<TestData.Contracts.StandardContract.TransferEvent>(blockchainUrl: BLOCKCHAIN_URL, contractAddresses: CONTRACT_ADDRESSES);
            Assert.Equal(CONTRACT_ADDRESSES, processor.Filters[0].Address);
        }

        [Fact]
        public void EventSpecific_Construction_WithFilterCustomisation()
        {
            string[] CONTRACT_ADDRESSES = new[] { "0x243e72b69141f6af525a9a5fd939668ee9f2b354", "0x343e72b69141f6af525a9a5fd939668ee9f2b354" };

            string desiredFromAddress = "0x943e72b69141f6af525a9a5fd939668ee9f2b354";

            var processor = new LogsProcessorBuilder<TestData.Contracts.StandardContract.TransferEvent>(
                blockchainUrl: BLOCKCHAIN_URL, (filterBuilder) => filterBuilder.AddTopic(f => f.From, desiredFromAddress));

            Assert.Equal("0x000000000000000000000000943e72b69141f6af525a9a5fd939668ee9f2b354", processor.Filters[0].GetFirstTopicValueAsString(1));
        }

        [Fact]
        public void EventSpecific_Construction_Contract_Address_WithFilterCustomisation()
        {
            string CONTRACT_ADDRESS = "0x243e72b69141f6af525a9a5fd939668ee9f2b354";

            string desiredFromAddress = "0x943e72b69141f6af525a9a5fd939668ee9f2b354";

            var processor = new LogsProcessorBuilder<TestData.Contracts.StandardContract.TransferEvent>(
                blockchainUrl: BLOCKCHAIN_URL, CONTRACT_ADDRESS, (filterBuilder) => filterBuilder.AddTopic(f => f.From, desiredFromAddress));

            Assert.Equal("0x000000000000000000000000943e72b69141f6af525a9a5fd939668ee9f2b354", processor.Filters[0].GetFirstTopicValueAsString(1));
            Assert.Equal(CONTRACT_ADDRESS, processor.Filters[0].Address[0]);
        }

        [Fact]
        public void EventSpecific_Construction_Contract_Addresses_WithFilterCustomisation()
        {
            string[] CONTRACT_ADDRESSES = new[] { "0x243e72b69141f6af525a9a5fd939668ee9f2b354", "0x343e72b69141f6af525a9a5fd939668ee9f2b354" };

            string desiredFromAddress = "0x943e72b69141f6af525a9a5fd939668ee9f2b354";

            var processor = new LogsProcessorBuilder<TestData.Contracts.StandardContract.TransferEvent>(
                blockchainUrl: BLOCKCHAIN_URL, CONTRACT_ADDRESSES, (filterBuilder) => filterBuilder.AddTopic(f => f.From, desiredFromAddress));

            Assert.Equal("0x000000000000000000000000943e72b69141f6af525a9a5fd939668ee9f2b354", processor.Filters[0].GetFirstTopicValueAsString(1));
            Assert.Equal(CONTRACT_ADDRESSES, processor.Filters[0].Address);
        }


        [Fact]
        public void Construction_FromUrl_CreatesBlockchainProxyService()
        {
            var processor = new LogsProcessorBuilder(blockchainUrl: BLOCKCHAIN_URL);
            Assert.NotNull(processor.BlockchainProxyService);
        }

        [Fact]
        public void Construction_FromWeb3_CreatesBlockchainProxyService()
        {
            var mockWeb3 = new Mock<IWeb3>();
            var processor = new LogsProcessorBuilder(mockWeb3.Object);
            Assert.NotNull(processor.BlockchainProxyService);
        }

        [Fact]
        public void Construction_AddsAFilterForContractAddress()
        {
            const string CONTRACT_ADDRESS = "0x243e72b69141f6af525a9a5fd939668ee9f2b354";
            var processor = new LogsProcessorBuilder(blockchainUrl: BLOCKCHAIN_URL, contractAddress: CONTRACT_ADDRESS);
            Assert.Single(processor.Filters);
            Assert.Equal(CONTRACT_ADDRESS, processor.Filters[0].Address[0]);
        }

        [Fact]
        public void Construction_AddsAFilterForContractAddresses()
        {
            string[] CONTRACT_ADDRESSES = new []{ "0x243e72b69141f6af525a9a5fd939668ee9f2b354", "0x343e72b69141f6af525a9a5fd939668ee9f2b354" };
            var processor = new LogsProcessorBuilder(blockchainUrl: BLOCKCHAIN_URL, contractAddresses: CONTRACT_ADDRESSES);
            Assert.Equal(CONTRACT_ADDRESSES, processor.Filters[0].Address);
        }

        [Fact]
        public void Construction_BlockchainProxyCanNotBeNull()
        {
            Assert.Throws<ArgumentNullException>(() => new LogsProcessorBuilder(blockchainProxyService:null));
        }

        [Fact]
        public void Construction_BlockchainProxyCanBePassed()
        {
            var mockProxy = new Mock<IBlockchainProxyService>();
            var processor = new LogsProcessorBuilder(mockProxy.Object);
            Assert.Same(mockProxy.Object, processor.BlockchainProxyService);
        }

        [Fact]
        public void Property_Processors_CanAmend()
        {
            var mockLogProcessor = new Mock<ILogProcessor>().Object;
            var processor = new LogsProcessorBuilder(blockchainUrl: BLOCKCHAIN_URL);
            processor.Processors.Add(mockLogProcessor);
        }

        [Fact]
        public void Property_Filters_CanAmend()
        {
            var processor = new LogsProcessorBuilder(blockchainUrl: BLOCKCHAIN_URL);
            processor.Filters.Add(new NewFilterInput());
        }

        [Fact]
        public void Method_Set_IsForChainingSetupCalls()
        {
            var processor = new LogsProcessorBuilder(blockchainUrl: BLOCKCHAIN_URL)
                .Set( p => p.BlocksPerBatch = 10)
                .Set(p => p.MinimumBlockNumber = 100000);
            
            Assert.Equal((uint)10, processor.BlocksPerBatch);
            Assert.Equal((ulong)100000, processor.MinimumBlockNumber);
        }

        [Fact]
        public async Task Add_WithCallBack_Creates_ExpectedProcessor()
        {
            bool callBackInvoked = false;

            var callback = new Action<IEnumerable<EventLog<TestData.Contracts.StandardContract.TransferEvent>>>(Logs =>
            {
                callBackInvoked = true;
            });

            var processor = new LogsProcessorBuilder(blockchainUrl: BLOCKCHAIN_URL)
                .Add(callback);

            var logProcessor = processor.Processors[0];
            var logsToProcess = new[] { TestData.Contracts.StandardContract.SampleTransferLog() };
            await logProcessor.ProcessLogsAsync(logsToProcess);

            Assert.True(callBackInvoked);
        }

        [Fact]
        public async Task Add_WithAsyncCallBack_Creates_ExpectedProcessor()
        {
            bool callBackInvoked = false;

            var callback = new Func<IEnumerable<EventLog<TestData.Contracts.StandardContract.TransferEvent>>, Task>(Logs =>
            {
                callBackInvoked = true;
                return Task.CompletedTask;
            });

            var processor = new LogsProcessorBuilder(blockchainUrl: BLOCKCHAIN_URL)
                .Add(callback);

            var logProcessor = processor.Processors[0];

            var logsToProcess = new[] { TestData.Contracts.StandardContract.SampleTransferLog() };
            await logProcessor.ProcessLogsAsync(logsToProcess);
            Assert.True(callBackInvoked);
        }

        [Fact]
        public async Task AddAndQueue_CreatesExpectedProcessor()
        {
            var queuedMessages = new List<object>();
            var mockQueue = new Mock<IQueue>();

            mockQueue.Setup(q => q.AddMessageAsync(It.IsAny<object>()))
                .Callback<object>(msg => queuedMessages.Add(msg))
                .Returns(Task.CompletedTask);

            var processor = new LogsProcessorBuilder(blockchainUrl: BLOCKCHAIN_URL)
                .AddToQueue<TestData.Contracts.StandardContract.TransferEvent>(mockQueue.Object);

            var logProcessor = processor.Processors[0];

            var logsToProcess = new[] { TestData.Contracts.StandardContract.SampleTransferLog() };
            await logProcessor.ProcessLogsAsync(logsToProcess);

            Assert.Single(queuedMessages);
            Assert.IsType<EventLog<TestData.Contracts.StandardContract.TransferEvent>>(queuedMessages.First());
        }

        [Fact]
        public async Task AddAndQueue_WithPredicate_CreatesExpectedProcessor()
        {
            var queuedMessages = new List<object>();
            var mockQueue = new Mock<IQueue>();

            mockQueue.Setup(q => q.AddMessageAsync(It.IsAny<object>()))
                .Callback<object>(msg => queuedMessages.Add(msg))
                .Returns(Task.CompletedTask);

            //set up with predicate that always returns false
            var processor = new LogsProcessorBuilder(blockchainUrl: BLOCKCHAIN_URL)
                .AddToQueue<TestData.Contracts.StandardContract.TransferEvent>(mockQueue.Object,
                predicate: (log) => false);

            var logProcessor = processor.Processors[0];

            var logsToProcess = new[] { TestData.Contracts.StandardContract.SampleTransferLog() };
            await logProcessor.ProcessLogsAsync(logsToProcess);

            Assert.Empty(queuedMessages);
        }

        public class QueueMessage
        {
            public object Content { get;set;}
        }

        [Fact]
        public async Task AddAndQueue_WithMapper_CreatesExpectedProcessor()
        {
            var queuedMessages = new List<object>();
            var mockQueue = new Mock<IQueue>();

            mockQueue.Setup(q => q.AddMessageAsync(It.IsAny<object>()))
                .Callback<object>(msg => queuedMessages.Add(msg))
                .Returns(Task.CompletedTask);

            var processor = new LogsProcessorBuilder(blockchainUrl: BLOCKCHAIN_URL)
                .AddToQueue<TestData.Contracts.StandardContract.TransferEvent>(mockQueue.Object,
                mapper: (log) => new QueueMessage{Content = log });

            var logProcessor = processor.Processors[0];

            var logsToProcess = new[] { TestData.Contracts.StandardContract.SampleTransferLog() };
            await logProcessor.ProcessLogsAsync(logsToProcess);

            Assert.IsType<QueueMessage>(queuedMessages.First());
        }

        [Fact]  
        public void Add_Passing_EventSubscription()
        {
            var transferSubscription = new EventSubscription<TestData.Contracts.StandardContract.TransferEvent>();
            var processor = new LogsProcessorBuilder(blockchainUrl: BLOCKCHAIN_URL)
                .Add(transferSubscription);
            Assert.Same(transferSubscription, processor.Processors.First());
        }

        [Fact]
        public void Add_Passing_Custom_LogProcessor()
        {
            var mockLogProcesor = new Mock<ILogProcessor>().Object;
            var processor = new LogsProcessorBuilder(blockchainUrl: BLOCKCHAIN_URL)
                .Add(mockLogProcesor);

            Assert.Same(mockLogProcesor, processor.Processors.First());
        }

        [Fact]
        public async Task Add_WithFilterLogCallBack_Creates_ExpectedProcessor()
        {
            var logsProcessed = new List<FilterLog>();

            var callback = new Action<IEnumerable<FilterLog>>(Logs =>
            {
                logsProcessed.AddRange(Logs);
            });

            var processor = new LogsProcessorBuilder(blockchainUrl: BLOCKCHAIN_URL)
                .Add(callback);

            var logProcessor = processor.Processors[0];
            var logsToProcess = new[] { new FilterLog() };
            await logProcessor.ProcessLogsAsync(logsToProcess);

            Assert.Equal(logsToProcess, logsProcessed);
        }

        [Fact]
        public async Task Add_WithFilterLogAsyncCallBack_Creates_ExpectedProcessor()
        {
            var logsProcessed = new List<FilterLog>();

            var callback = new Func<IEnumerable<FilterLog>, Task>(Logs =>
            {
                logsProcessed.AddRange(Logs);
                return Task.CompletedTask;
            });

            var processor = new LogsProcessorBuilder(blockchainUrl: BLOCKCHAIN_URL)
                .Add(callback);

            var logProcessor = processor.Processors[0];

            var logsToProcess = new[] { new FilterLog() };
            await logProcessor.ProcessLogsAsync(logsToProcess);

            Assert.Equal(logsToProcess, logsProcessed);
        }

        [Fact]
        public async Task AddAndQueue_ForFilterLog_CreatesExpectedProcessor()
        {
            var queuedMessages = new List<object>();
            var mockQueue = new Mock<IQueue>();

            mockQueue.Setup(q => q.AddMessageAsync(It.IsAny<object>()))
                .Callback<object>(msg => queuedMessages.Add(msg))
                .Returns(Task.CompletedTask);

            //set up with a predicate that always returns false
            var processor = new LogsProcessorBuilder(blockchainUrl: BLOCKCHAIN_URL)
                .AddToQueue(mockQueue.Object);

            var logProcessor = processor.Processors[0];

            var logsToProcess = new[] { new FilterLog() };
            await logProcessor.ProcessLogsAsync(logsToProcess);

            Assert.Single(queuedMessages);
            Assert.Same(logsToProcess[0], queuedMessages[0]);
        }

        [Fact]
        public async Task AddAndQueue_ForFilterLog_WithPredicate_CreatesExpectedProcessor()
        {
            var queuedMessages = new List<object>();
            var mockQueue = new Mock<IQueue>();

            mockQueue.Setup(q => q.AddMessageAsync(It.IsAny<object>()))
                .Callback<object>(msg => queuedMessages.Add(msg))
                .Returns(Task.CompletedTask);

            //set up with a predicate that always returns false
            var processor = new LogsProcessorBuilder(blockchainUrl: BLOCKCHAIN_URL)
                .AddToQueue(mockQueue.Object,
                predicate: (logs) => false);

            var logProcessor = processor.Processors[0];

            var logsToProcess = new[] { new FilterLog() };
            await logProcessor.ProcessLogsAsync(logsToProcess);

            Assert.Empty(queuedMessages);
        }

        [Fact]
        public async Task AddAndQueue_ForFilterLog_WithMapper_CreatesExpectedProcessor()
        {
            var queuedMessages = new List<object>();
            var mockQueue = new Mock<IQueue>();

            mockQueue.Setup(q => q.AddMessageAsync(It.IsAny<object>()))
                .Callback<object>(msg => queuedMessages.Add(msg))
                .Returns(Task.CompletedTask);

            var processor = new LogsProcessorBuilder(blockchainUrl: BLOCKCHAIN_URL)
                .AddToQueue(mockQueue.Object,
                mapper: (log) => new QueueMessage { Content = log });

            var logProcessor = processor.Processors[0];

            var logsToProcess = new[] { new FilterLog() };
            await logProcessor.ProcessLogsAsync(logsToProcess);

            Assert.IsType<QueueMessage>(queuedMessages.First());
        }

        [Fact]
        public async Task ProcessOnceAsync()
        {            
            var mockLogProcessor = new Mock<ILogProcessor>();
            var mockBlockchainProxy = new Mock<IBlockchainProxyService>();
            var logs = new[] { new FilterLog() };

            mockBlockchainProxy
                .Setup(p => p.GetLogs(It.IsAny<NewFilterInput>(), null))
                .ReturnsAsync(logs);

            mockBlockchainProxy
                .Setup(p => p.GetMaxBlockNumberAsync())
                .ReturnsAsync((ulong)100);

            var builder = new LogsProcessorBuilder(mockBlockchainProxy.Object)
                .Set(p => p.BlocksPerBatch = 1)
                .Set(p => p.MinimumBlockNumber = 0)
                .Add(mockLogProcessor.Object);

            var processor = builder.Build();

            var result = await processor.ProcessOnceAsync(new System.Threading.CancellationToken());

            Assert.Equal((ulong)0, result.Value.From);
            Assert.Equal((ulong)0, result.Value.To);

            result = await processor.ProcessOnceAsync(new System.Threading.CancellationToken());

            Assert.Equal((ulong)1, result.Value.From);
            Assert.Equal((ulong)1, result.Value.To);

            Assert.Equal((ulong)1, await builder.BlockProgressRepository.GetLastBlockNumberProcessedAsync());
        }

        [Fact]
        public async Task ProcessContinuallyAsync_Using_OnBatchProcessed()
        {
            (ulong batchCount, BlockRange lastRange)? lastBatchProcessedArgs = null;

            var mockLogProcessor = new Mock<ILogProcessor>();
            var mockBlockchainProxy = new Mock<IBlockchainProxyService>();
            var logs = new[] { new FilterLog() };

            mockBlockchainProxy
                .Setup(p => p.GetLogs(It.IsAny<NewFilterInput>(), null))
                .ReturnsAsync(logs);

            mockBlockchainProxy
                .Setup(p => p.GetMaxBlockNumberAsync())
                .ReturnsAsync((ulong)100);

            var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(10));

            var builder = new LogsProcessorBuilder(mockBlockchainProxy.Object)
                .Set(p => p.BlocksPerBatch = 1)
                .Set(p => p.MinimumBlockNumber = 0)
                // escape hatch
                .OnBatchProcessed((batchCount, lastRange) => 
                { 
                    lastBatchProcessedArgs = (batchCount, lastRange); 
                    cancellationTokenSource.Cancel();
                })
                .Add(mockLogProcessor.Object);

            var processor = builder.Build();

            var rangesProcessed = await processor.ProcessContinuallyAsync(cancellationTokenSource.Token);

            Assert.Equal((ulong)1, rangesProcessed);
            Assert.Equal((ulong)1, lastBatchProcessedArgs.Value.batchCount);
            Assert.Equal((ulong)0, lastBatchProcessedArgs.Value.lastRange.From);
            Assert.Equal((ulong)0, lastBatchProcessedArgs.Value.lastRange.To);

            Assert.Equal((ulong)0, await builder.BlockProgressRepository.GetLastBlockNumberProcessedAsync());
        }

        [Fact]
        public async Task RunInBackgroundAsync_Using_OnBatchProcessed()
        {
            (ulong batchCount, BlockRange lastRange)? lastBatchProcessedArgs = null;

            var mockLogProcessor = new Mock<ILogProcessor>();
            var mockBlockchainProxy = new Mock<IBlockchainProxyService>();
            var logs = new[] { new FilterLog() };

            mockBlockchainProxy
                .Setup(p => p.GetLogs(It.IsAny<NewFilterInput>(), null))
                .ReturnsAsync(logs);

            mockBlockchainProxy
                .Setup(p => p.GetMaxBlockNumberAsync())
                .ReturnsAsync((ulong)100);

            var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(10));

            var builder = new LogsProcessorBuilder(mockBlockchainProxy.Object)
                .Set(p => p.BlocksPerBatch = 1)
                .Set(p => p.MinimumBlockNumber = 0)
                // escape hatch
                .OnBatchProcessed((batchCount, lastRange) =>
                {
                    lastBatchProcessedArgs = (batchCount, lastRange);
                    cancellationTokenSource.Cancel();
                })
                .Add(mockLogProcessor.Object);

            var processor = builder.Build();

            var backgroundProcessingTask = processor.ProcessContinuallyInBackgroundAsync(cancellationTokenSource.Token);

            //simulate doing some work until cancellation is requested
            while (!backgroundProcessingTask.IsCompleted)
            {
                await Task.Delay(10);
            }

            Assert.Equal((ulong)1, lastBatchProcessedArgs.Value.batchCount);
            Assert.Equal((ulong)0, lastBatchProcessedArgs.Value.lastRange.From);
            Assert.Equal((ulong)0, lastBatchProcessedArgs.Value.lastRange.To);

            Assert.Equal((ulong)0, await builder.BlockProgressRepository.GetLastBlockNumberProcessedAsync());
        }

        [Fact]
        public async Task IfMinimumBlockNumberIsNull_AndThereIsNoPreviousProgress_StartsAtCurrentBlockOnChain()
        {
            var mockLogProcessor = new Mock<ILogProcessor>();
            var mockBlockchainProxy = new Mock<IBlockchainProxyService>();
            var logs = new[] { new FilterLog() };

            mockBlockchainProxy
                .Setup(p => p.GetLogs(It.IsAny<NewFilterInput>(), null))
                .ReturnsAsync(logs);

            mockBlockchainProxy
                .Setup(p => p.GetMaxBlockNumberAsync())
                .ReturnsAsync((ulong)100);

            var processor = new LogsProcessorBuilder(mockBlockchainProxy.Object)
                .Set(p => p.BlocksPerBatch = 1)
                .Add(mockLogProcessor.Object)
                .Build();

            var result = await processor.ProcessOnceAsync(new System.Threading.CancellationToken());

            Assert.Equal((ulong)100, result.Value.From);
            Assert.Equal((ulong)100, result.Value.To);
        }

        [Fact]
        public void UseBlockProgressRepository()
        {
            var mockProgressRepository = new Mock<IBlockProgressRepository>().Object;
            var builder = new LogsProcessorBuilder(blockchainUrl: BLOCKCHAIN_URL)
                .UseBlockProgressRepository(mockProgressRepository);
            Assert.Same(mockProgressRepository, builder.BlockProgressRepository);
        }

        [Fact]
        public void UseJsonFileForBlockProgress()
        {
            var jsonFilePath = Path.Combine(Path.GetTempPath(),  $"{Guid.NewGuid()}.json");
            var processor = new LogsProcessorBuilder(blockchainUrl: BLOCKCHAIN_URL)
                .UseJsonFileForBlockProgress(jsonFilePath);
            Assert.IsType<JsonBlockProgressRepository>(processor.BlockProgressRepository);

            File.Delete(jsonFilePath);
        }

        [Fact]
        public void Filter()
        {
            var filter = new NewFilterInput();

            var processor = new LogsProcessorBuilder(blockchainUrl: BLOCKCHAIN_URL)
                .Filter(filter);

            Assert.Same(filter, processor.Filters[0]);
        }

        [Fact]
        public void Filter_AddsAnEventSignatureBasedFilter()
        {
            var processor = new LogsProcessorBuilder(blockchainUrl: BLOCKCHAIN_URL)
                .Filter<TestData.Contracts.StandardContract.TransferEvent>();

            Assert.Equal(
                ABITypedRegistry
                .GetEvent<TestData.Contracts.StandardContract.TransferEvent>()
                .Sha3Signature.EnsureHexPrefix(),
                processor.Filters[0].Topics[0]);

        }

        [Fact]
        public void Filter_SetsAnEventSignatureBasedFilter_WithContractAddresses()
        {
            string[] CONTRACT_ADDRESSES = new[] { "0x243e72b69141f6af525a9a5fd939668ee9f2b354", "0x343e72b69141f6af525a9a5fd939668ee9f2b354" };

            var processor = new LogsProcessorBuilder(blockchainUrl: BLOCKCHAIN_URL, contractAddresses: CONTRACT_ADDRESSES)
                .Filter<TestData.Contracts.StandardContract.TransferEvent>();

            //should override and replace contract address filter with a new filter for contract address and event
            Assert.Single(processor.Filters);

            Assert.Equal(
                ABITypedRegistry
                .GetEvent<TestData.Contracts.StandardContract.TransferEvent>()
                .Sha3Signature.EnsureHexPrefix(),
                processor.Filters[0].Topics[0]);


            Assert.Equal(CONTRACT_ADDRESSES, processor.Filters[0].Address);
        }


        [Fact]
        public async Task WillRequestLogsForEachFilter()
        {
            var mockLogProcessor = new Mock<ILogProcessor>();
            var mockBlockchainProxy = new Mock<IBlockchainProxyService>();
            var logs = new[] { new FilterLog() };
            var filtersExecuted = new List<NewFilterInput>();

            var filters = new[] {new NewFilterInput(), new NewFilterInput()};

            mockBlockchainProxy
                .Setup(p => p.GetLogs(It.IsAny<NewFilterInput>(), null))
                .Callback<NewFilterInput, object>((f,o) => filtersExecuted.Add(f))
                .ReturnsAsync(logs);

            mockBlockchainProxy
                .Setup(p => p.GetMaxBlockNumberAsync())
                .ReturnsAsync((ulong)100);

            var processor = new LogsProcessorBuilder(mockBlockchainProxy.Object)
                .Set(p => p.BlocksPerBatch = 1)
                .Filter(filters[0])
                .Filter(filters[1])
                .Add(mockLogProcessor.Object)
                .Build();

            var result = await processor.ProcessOnceAsync(new System.Threading.CancellationToken());

            Assert.Equal(filters, filtersExecuted);
        }

        [Fact]
        public async Task RunInBackgroundAsync_Using_OnFatalError()
        {
            Exception fakeException = new Exception();
            Exception fatalError = null;

            //set up processor to throw 
            var mockLogProcessor = new Mock<ILogProcessor>();
            mockLogProcessor
                .Setup(s => s.ProcessLogsAsync(It.IsAny<FilterLog[]>()))
                .ThrowsAsync(fakeException);
            
                
            var mockBlockchainProxy = new Mock<IBlockchainProxyService>();
            var logs = new[] { new FilterLog() };

            mockBlockchainProxy
                .Setup(p => p.GetLogs(It.IsAny<NewFilterInput>(), null))
                .ReturnsAsync(logs);

            mockBlockchainProxy
                .Setup(p => p.GetMaxBlockNumberAsync())
                .ReturnsAsync((ulong)100);

            var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(10));

            var processor = new LogsProcessorBuilder(mockBlockchainProxy.Object)
                .Set(p => p.BlocksPerBatch = 1)
                .Set(p => p.MinimumBlockNumber = 0)
                .OnFatalError(e => fatalError = e)
                .Add(mockLogProcessor.Object)
                .Build();

            var backgroundTask = processor.ProcessContinuallyInBackgroundAsync(cancellationTokenSource.Token);

            //simulate doing some work until cancellation is requested
            while (!backgroundTask.IsCompleted)
            {
                await Task.Delay(100);
            }

            Assert.Same(fakeException, fatalError);
        }

        [Fact]
        public void DisposeOnProcessorDisposing_RegistersDisposableObject()
        {
            var disposed = false;
            var disposable = new Mock<IDisposable>();
            disposable.Setup(d => d.Dispose()).Callback(() => disposed = true);

            using(var processor = new LogsProcessorBuilder(blockchainUrl: BLOCKCHAIN_URL)
                .Add<TestData.Contracts.StandardContract.TransferEvent>((transfers) => { })
                .DisposeOnProcessorDisposing(disposable.Object)
                .Build())
            {}

            Assert.True(disposed);
        }

    }
}
