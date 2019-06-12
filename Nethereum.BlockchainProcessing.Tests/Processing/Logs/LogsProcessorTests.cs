using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using Nethereum.BlockchainProcessing.BlockchainProxy;
using Nethereum.BlockchainProcessing.Processing;
using Nethereum.LogProcessing;
using Xunit;

namespace Nethereum.BlockchainProcessing.Tests.Processing.Logs
{
    public class LogsProcessorTests
    {
        const uint MaxBlocksPerBatch = 10;
        protected internal Mock<IBlockchainProcessor> MockProcessor;
        protected internal Mock<IBlockProgressService> MockProgressService;
        protected internal LogsProcessor Service;

        public LogsProcessorTests()
        {
            MockProcessor = new Mock<IBlockchainProcessor>();
            MockProgressService = new Mock<IBlockProgressService>();
            Service = new LogsProcessor(
                MockProcessor.Object, MockProgressService.Object, MaxBlocksPerBatch);
        }

        public class ProcessAsyncTests : LogsProcessorTests
        {
            [Fact]
            public async Task When_There_Is_Nothing_To_Process_Returns_Null()
            {
                MockProgressService
                    .Setup(s => s.GetNextBlockRangeToProcessAsync(MaxBlocksPerBatch))
                    .ReturnsAsync((BlockRange?) null);

                var processedRange = await Service.ProcessOnceAsync();

                Assert.Null(processedRange);
                MockProcessor.Verify(p => p.ProcessAsync(It.IsAny<BlockRange>()), Times.Never);
            }

            [Fact]
            public async Task Requests_Range_Processes_And_Records_Progress()
            {
                var range = new BlockRange(0, 10);

                MockProgressService
                    .Setup(s => s.GetNextBlockRangeToProcessAsync(MaxBlocksPerBatch))
                    .ReturnsAsync(range);

                MockProcessor
                    .Setup(p => p.ProcessAsync(range, It.IsAny<CancellationToken>()))
                    .Returns(Task.CompletedTask);

                MockProgressService
                    .Setup(s => s.SaveLastBlockProcessedAsync(range.To))
                    .Returns(Task.CompletedTask);

                var processedRange = await Service.ProcessOnceAsync();

                Assert.Equal(range, processedRange);

                MockProcessor.Verify();
                MockProgressService.Verify();
            }

            [Fact]
            public async Task Catches_Too_Many_Records_Exception_And_Retries_With_Reduced_Batch_Size()
            {
                var largeRange = new BlockRange(0, 10);
                //expect the batch size to be reduced by half
                var smallerRange = new BlockRange(0, 5);

                //expect max of 10 blocks to be requested
                MockProgressService
                    .Setup(s => s.GetNextBlockRangeToProcessAsync(MaxBlocksPerBatch))
                    .ReturnsAsync(largeRange);

                // mock a too-many-records exception
                MockProcessor
                    .Setup(p => p.ProcessAsync(largeRange, It.IsAny<CancellationToken>()))
                    .ThrowsAsync(new TooManyRecordsException());

                //expect max of 5 blocks to be requested
                MockProgressService
                    .Setup(s => s.GetNextBlockRangeToProcessAsync(5))
                    .ReturnsAsync(smallerRange);

                // mock a successfull process attempt
                MockProcessor
                    .Setup(p => p.ProcessAsync(smallerRange, It.IsAny<CancellationToken>()))
                    .Returns(Task.CompletedTask);

                // expect progress to be updated
                MockProgressService
                    .Setup(s => s.SaveLastBlockProcessedAsync(smallerRange.To))
                    .Returns(Task.CompletedTask);

                var processedRange = await Service.ProcessOnceAsync();

                Assert.Equal(smallerRange, processedRange);
                Assert.Equal((ulong)5, Service.MaxNumberOfBlocksPerBatch);

                MockProcessor.Verify();
                MockProgressService.Verify();
            }

            [Fact]
            public async Task Catches_Too_Many_Records_Exception_Will_Throw_If_Max_Blocks_Reaches_One()
            {
                var largeRange = new BlockRange(0, 10);
                var rangesAttempted = new List<BlockRange>();

                MockProgressService
                    .Setup(s => s.GetNextBlockRangeToProcessAsync(It.IsAny<uint>()))
                    .Returns<uint>((newMax) => Task.FromResult(new BlockRange?(new BlockRange(0, newMax))));

                // mock a too-many-records exception for every call
                MockProcessor
                    .Setup(p => p.ProcessAsync(It.IsAny<BlockRange>(), It.IsAny<CancellationToken>()))
                    .Callback<BlockRange, CancellationToken>((range, token) => rangesAttempted.Add(range))
                    .ThrowsAsync(new TooManyRecordsException());

                await Assert.ThrowsAsync<TooManyRecordsException>(async () => await Service.ProcessOnceAsync());

                Assert.Equal((ulong)10, rangesAttempted[0].To);
                Assert.Equal((ulong)5, rangesAttempted[1].To);
                Assert.Equal((ulong)2, rangesAttempted[2].To);
                Assert.Equal((ulong)1, rangesAttempted[3].To);

                Assert.Equal((ulong)1, Service.MaxNumberOfBlocksPerBatch);
            }
        }

        public class ProcessContinuallyAsyncTests : LogsProcessorTests
        {
            [Fact]
            public async Task Processes_Ranges_And_Waits_Until_Next_Range_Is_Available()
            {
                var mockWaitStategy = new Mock<IWaitStrategy>();
                Service.WaitForBlockStrategy = mockWaitStategy.Object;

                var waits = new List<uint>();
                var cancellationSource = new CancellationTokenSource();

                var range1 = new BlockRange(0, 9);
                var nullRange = new BlockRange?();
                var range2 = new BlockRange(10, 19);

                var rangeQueue = new Queue<BlockRange?>();
                //initial range to process
                rangeQueue.Enqueue(range1);

                //simulate being up to date for 3 iterations
                rangeQueue.Enqueue(nullRange);
                rangeQueue.Enqueue(nullRange);
                rangeQueue.Enqueue(nullRange);
                
                //simulate new range becoming available
                rangeQueue.Enqueue(range2);

                //get next range from queue
                MockProgressService
                    .Setup(s => s.GetNextBlockRangeToProcessAsync(MaxBlocksPerBatch))
                    .ReturnsAsync(() => rangeQueue.Dequeue());

                //process range 1
                MockProcessor
                    .Setup(p => p.ProcessAsync(range1, It.IsAny<CancellationToken>()))
                    .Returns(Task.CompletedTask);

                //when range is null expect wait strategy to be invoked
                mockWaitStategy
                    .Setup(s => s.Apply(It.IsAny<uint>()))
                    .Callback<uint>((attemptCount) => waits.Add(attemptCount))
                    .Returns(Task.CompletedTask);

                //process range 2
                MockProcessor
                    .Setup(p => p.ProcessAsync(range2, It.IsAny<CancellationToken>()))
                    .Returns(Task.CompletedTask);

                //update progress range 1
                MockProgressService
                    .Setup(s => s.SaveLastBlockProcessedAsync(range1.To))
                    .Returns(Task.CompletedTask);

                //update progress range 2
                MockProgressService
                    .Setup(s => s.SaveLastBlockProcessedAsync(range2.To))
                    .Returns(Task.CompletedTask);

                //short circuit callback used to exit process
                var shortCircuit = new Action<uint, BlockRange>((rangesProcessed, lastRange) =>
                {
                    if(lastRange.Equals(range2)) cancellationSource.Cancel();
                });

                var blocksProcessed = await Service.ProcessContinuallyAsync(
                    cancellationSource.Token, shortCircuit);

                Assert.Equal((ulong)20, blocksProcessed);
                Assert.Equal(3, waits.Count);
                //wait strategy is sent an attempt count so it can adjust wait time accordingly 
                Assert.True(waits.SequenceEqual(new uint[]{1,2,3}));

                MockProcessor.Verify();
                MockProgressService.Verify();
            }
        }
    }
}
