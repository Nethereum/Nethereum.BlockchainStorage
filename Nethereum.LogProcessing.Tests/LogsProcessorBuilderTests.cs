using Moq;
using Nethereum.BlockchainProcessing.Common.Tests;
using Nethereum.BlockchainProcessing.Processing;
using Nethereum.BlockchainProcessing.Processing.Logs;
using Nethereum.BlockchainProcessing.Processing.Logs.Handling;
using Nethereum.Contracts;
using Nethereum.Hex.HexConvertors.Extensions;
using Nethereum.LogProcessing;
using Nethereum.RPC.Eth.DTOs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Nethereum.LogProcessing.Tests
{
    public class LogsProcessorBuilderTests
    {
        const string BLOCKCHAIN_URL = "http://localhost:8545/";

        [Fact]
        public void EventSpecific_Construction_FromUrl_CreatesEthApiContractService()
        {
            var processor = new LogsProcessorBuilder<TestData.Contracts.StandardContract.TransferEvent>(blockchainUrl: BLOCKCHAIN_URL);
            Assert.NotNull(processor.Eth);
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
                blockchainUrl: BLOCKCHAIN_URL, configureFilterBuilder: (filterBuilder) => filterBuilder.AddTopic(f => f.From, desiredFromAddress));

            Assert.Equal("0x000000000000000000000000943e72b69141f6af525a9a5fd939668ee9f2b354", processor.Filters[0].GetFirstTopicValueAsString(1));
        }

        [Fact]
        public void EventSpecific_Construction_Contract_Address_WithFilterCustomisation()
        {
            string CONTRACT_ADDRESS = "0x243e72b69141f6af525a9a5fd939668ee9f2b354";

            string desiredFromAddress = "0x943e72b69141f6af525a9a5fd939668ee9f2b354";

            var processor = new LogsProcessorBuilder<TestData.Contracts.StandardContract.TransferEvent>(
                blockchainUrl: BLOCKCHAIN_URL,
                contractAddress: CONTRACT_ADDRESS,
                configureFilterBuilder: (filterBuilder) => filterBuilder.AddTopic(f => f.From, desiredFromAddress));

            Assert.Equal("0x000000000000000000000000943e72b69141f6af525a9a5fd939668ee9f2b354", processor.Filters[0].GetFirstTopicValueAsString(1));
            Assert.Equal(CONTRACT_ADDRESS, processor.Filters[0].Address[0]);
        }

        [Fact]
        public void EventSpecific_Construction_Contract_Addresses_WithFilterCustomisation()
        {
            string[] CONTRACT_ADDRESSES = new[] { "0x243e72b69141f6af525a9a5fd939668ee9f2b354", "0x343e72b69141f6af525a9a5fd939668ee9f2b354" };

            string desiredFromAddress = "0x943e72b69141f6af525a9a5fd939668ee9f2b354";

            var processor = new LogsProcessorBuilder<TestData.Contracts.StandardContract.TransferEvent>(
                blockchainUrl: BLOCKCHAIN_URL,
                contractAddresses: CONTRACT_ADDRESSES,
                configureFilterBuilder: (filterBuilder) => filterBuilder.AddTopic(f => f.From, desiredFromAddress));

            Assert.Equal("0x000000000000000000000000943e72b69141f6af525a9a5fd939668ee9f2b354", processor.Filters[0].GetFirstTopicValueAsString(1));
            Assert.Equal(CONTRACT_ADDRESSES, processor.Filters[0].Address);
        }


        [Fact]
        public void Construction_FromUrl_CreatesEthApiContractService()
        {
            var processor = new LogsProcessorBuilder(blockchainUrl: BLOCKCHAIN_URL);
            Assert.NotNull(processor.Eth);
        }

        [Fact]
        public void Construction_FromWeb3_CreatesEthApiContractService()
        {
            var mockWeb3 = new Web3Mock();
            var processor = new LogsProcessorBuilder(mockWeb3.Web3);
            Assert.NotNull(processor.Eth);
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
            string[] CONTRACT_ADDRESSES = new[] { "0x243e72b69141f6af525a9a5fd939668ee9f2b354", "0x343e72b69141f6af525a9a5fd939668ee9f2b354" };
            var processor = new LogsProcessorBuilder(blockchainUrl: BLOCKCHAIN_URL, contractAddresses: CONTRACT_ADDRESSES);
            Assert.Equal(CONTRACT_ADDRESSES, processor.Filters[0].Address);
        }

        [Fact]
        public void Construction_BlockchainProxyCanNotBeNull()
        {
            Assert.Throws<ArgumentNullException>(() => new LogsProcessorBuilder(eth: null));
        }

        [Fact]
        public void Construction_BlockchainProxyCanBePassed()
        {
            var mockProxy = new Web3Mock();
            var processor = new LogsProcessorBuilder(mockProxy.Eth);
            Assert.Same(mockProxy.Eth, processor.Eth);
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
                .Set(p => p.BlocksPerBatch = 10)
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
            public object Content { get; set; }
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
                mapper: (log) => new QueueMessage { Content = log });

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
        public async Task Add_Predicate_With_Func_Creates_ExpectedProcessor()
        {
            var logsProcessed = new List<FilterLog>();
            var logsQueried = new List<FilterLog>();

            var predicate = new Predicate<FilterLog>((log) =>
            {
                logsQueried.Add(log);
                return true;
            });

            var callback = new Func<IEnumerable<FilterLog>, Task>(Logs =>
            {
                logsProcessed.AddRange(Logs);
                return Task.CompletedTask;
            });

            var processor = new LogsProcessorBuilder(blockchainUrl: BLOCKCHAIN_URL)
                .Add(predicate, callback);

            var logProcessor = processor.Processors[0];

            var logsToProcess = new[] { new FilterLog() };

            foreach (var log in logsToProcess) logProcessor.IsLogForEvent(log);
            await logProcessor.ProcessLogsAsync(logsToProcess);

            Assert.Equal(logsToProcess, logsQueried);
            Assert.Equal(logsToProcess, logsProcessed);
        }

        [Fact]
        public async Task Add_Predicate_With_Action_Creates_ExpectedProcessor()
        {
            var logsProcessed = new List<FilterLog>();
            var logsQueried = new List<FilterLog>();

            var predicate = new Predicate<FilterLog>((log) =>
            {
                logsQueried.Add(log);
                return true;
            });

            var action = new Action<IEnumerable<FilterLog>>(Logs =>
            {
                logsProcessed.AddRange(Logs);
            });

            var processor = new LogsProcessorBuilder(blockchainUrl: BLOCKCHAIN_URL)
                .Add(predicate, action);

            var logProcessor = processor.Processors[0];

            var logsToProcess = new[] { new FilterLog() };
            foreach (var log in logsToProcess) logProcessor.IsLogForEvent(log);
            await logProcessor.ProcessLogsAsync(logsToProcess);

            Assert.Equal(logsToProcess, logsQueried);
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
            var web3Mock = new Web3Mock();
            var logs = new[] { new FilterLog() };

            web3Mock.GetLogsMock
                .Setup(p => p.SendRequestAsync(It.IsAny<NewFilterInput>(), null))
                .ReturnsAsync(logs);

            web3Mock.BlockNumberMock
                .Setup(p => p.SendRequestAsync(null))
                .ReturnsAsync(100.ToHexBigInteger());

            var builder = new LogsProcessorBuilder(web3Mock.Mock.Object.Eth)
                .Set(p => p.BlocksPerBatch = 1)
                .Set(p => p.MinimumBlockNumber = 0)
                .Add(mockLogProcessor.Object);

            var processor = builder.Build();

            var result = await processor.ProcessOnceAsync(new CancellationToken());

            Assert.Equal(0, result.Value.From.Value);
            Assert.Equal(0, result.Value.To.Value);

            result = await processor.ProcessOnceAsync(new CancellationToken());

            Assert.Equal(1, result.Value.From.Value);
            Assert.Equal(1, result.Value.To.Value);

            Assert.Equal((ulong)1, await builder.BlockProgressRepository.GetLastBlockNumberProcessedAsync());
        }

        [Fact]
        public async Task ProcessContinuallyAsync_Using_OnBatchProcessed()
        {
            LogBatchProcessedArgs lastBatchProcessedArgs = null;

            var mockLogProcessor = new Mock<ILogProcessor>();
            var web3Mock = new Web3Mock();
            var logs = new[] { new FilterLog() };

            web3Mock.GetLogsMock
                .Setup(p => p.SendRequestAsync(It.IsAny<NewFilterInput>(), null))
                .ReturnsAsync(logs);

            web3Mock.BlockNumberMock
                .Setup(p => p.SendRequestAsync(null))
                .ReturnsAsync(100.ToHexBigInteger());

            var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(10));

            var builder = new LogsProcessorBuilder(web3Mock.ContractServiceMock.Object)
                .Set(p => p.BlocksPerBatch = 1)
                .Set(p => p.MinimumBlockNumber = 0)
                // escape hatch
                .OnBatchProcessed((args) =>
                {
                    lastBatchProcessedArgs = args;
                    cancellationTokenSource.Cancel();
                })
                .Add(mockLogProcessor.Object);

            var processor = builder.Build();

            var rangesProcessed = await processor.ProcessContinuallyAsync(cancellationTokenSource.Token);

            Assert.Equal(1, rangesProcessed);
            Assert.Equal((uint)1, lastBatchProcessedArgs.BatchesProcessedSoFar);
            Assert.Equal(0, lastBatchProcessedArgs.LastRangeProcessed.From.Value);
            Assert.Equal(0, lastBatchProcessedArgs.LastRangeProcessed.To.Value);

            Assert.Equal(0, await builder.BlockProgressRepository.GetLastBlockNumberProcessedAsync());
        }

        [Fact]
        public async Task RunInBackgroundAsync_Using_OnBatchProcessed()
        {
            LogBatchProcessedArgs lastBatchProcessedArgs = null;

            var mockLogProcessor = new Mock<ILogProcessor>();
            var web3Mock = new Web3Mock();
            var logs = new[] { new FilterLog() };

            web3Mock.GetLogsMock
                .Setup(p => p.SendRequestAsync(It.IsAny<NewFilterInput>(), null))
                .ReturnsAsync(logs);

            web3Mock.BlockNumberMock
                .Setup(p => p.SendRequestAsync(null))
                .ReturnsAsync(100.ToHexBigInteger());

            var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(10));

            var builder = new LogsProcessorBuilder(web3Mock.Eth)
                .Set(p => p.BlocksPerBatch = 1)
                .Set(p => p.MinimumBlockNumber = 0)
                // escape hatch
                .OnBatchProcessed((args) =>
                {
                    lastBatchProcessedArgs = args;
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

            Assert.Equal((uint)1, lastBatchProcessedArgs.BatchesProcessedSoFar);
            Assert.Equal(0, lastBatchProcessedArgs.LastRangeProcessed.From.Value);
            Assert.Equal(0, lastBatchProcessedArgs.LastRangeProcessed.To.Value);

            Assert.Equal(0, await builder.BlockProgressRepository.GetLastBlockNumberProcessedAsync());
        }

        [Fact]
        public async Task IfMinimumBlockNumberIsNull_AndThereIsNoPreviousProgress_StartsAtCurrentBlockOnChain()
        {
            var mockLogProcessor = new Mock<ILogProcessor>();
            var web3Mock = new Web3Mock();
            var logs = new[] { new FilterLog() };

            web3Mock.GetLogsMock
                .Setup(p => p.SendRequestAsync(It.IsAny<NewFilterInput>(), null))
                .ReturnsAsync(logs);

            web3Mock.BlockNumberMock
                .Setup(p => p.SendRequestAsync(null))
                .ReturnsAsync(100.ToHexBigInteger());

            var processor = new LogsProcessorBuilder(web3Mock.Eth)
                .Set(p => p.BlocksPerBatch = 1)
                .Add(mockLogProcessor.Object)
                .Build();

            var result = await processor.ProcessOnceAsync(new CancellationToken());

            Assert.Equal(100.ToHexBigInteger(), result.Value.From);
            Assert.Equal(100.ToHexBigInteger(), result.Value.To);
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
            var jsonFilePath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.json");
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
            var web3Mock = new Web3Mock();
            var logs = new[] { new FilterLog() };
            var filtersExecuted = new List<NewFilterInput>();

            var filters = new[] { new NewFilterInput(), new NewFilterInput() };

            web3Mock.GetLogsMock
                .Setup(p => p.SendRequestAsync(It.IsAny<NewFilterInput>(), null))
                .Callback<NewFilterInput, object>((f, o) => filtersExecuted.Add(f))
                .ReturnsAsync(logs);

            web3Mock.BlockNumberMock
                .Setup(p => p.SendRequestAsync(null))
                .ReturnsAsync(100.ToHexBigInteger());

            var processor = new LogsProcessorBuilder(web3Mock.Eth)
                .Set(p => p.BlocksPerBatch = 1)
                .Filter(filters[0])
                .Filter(filters[1])
                .Add(mockLogProcessor.Object)
                .Build();

            var result = await processor.ProcessOnceAsync(new CancellationToken());

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


            var web3Mock = new Web3Mock();
            var logs = new[] { new FilterLog() };

            web3Mock.GetLogsMock
                .Setup(p => p.SendRequestAsync(It.IsAny<NewFilterInput>(), null))
                .ReturnsAsync(logs);

            web3Mock.BlockNumberMock
                .Setup(p => p.SendRequestAsync(null))
                .ReturnsAsync(100.ToHexBigInteger());

            var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(10));

            var processor = new LogsProcessorBuilder(web3Mock.Eth)
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

            using (var processor = new LogsProcessorBuilder(blockchainUrl: BLOCKCHAIN_URL)
                .Add<TestData.Contracts.StandardContract.TransferEvent>((transfers) => { })
                .DisposeOnProcessorDisposing(disposable.Object)
                .Build())
            { }

            Assert.True(disposed);
        }

    }
}
