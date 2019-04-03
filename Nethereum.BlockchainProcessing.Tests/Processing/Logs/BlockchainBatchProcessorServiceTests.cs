using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using Nethereum.BlockchainProcessing.Processing;
using Xunit;

namespace Nethereum.BlockchainProcessing.Tests.Processing.Logs
{
    public class BlockchainBatchProcessorServiceTests
    {
        const uint MaxBlocksPerBatch = 10;
        protected internal Mock<IBlockchainProcessor> MockProcessor;
        protected internal Mock<IBlockProgressService> MockProgressService;
        protected internal BlockchainBatchProcessorService Service;

        public BlockchainBatchProcessorServiceTests()
        {
            MockProcessor = new Mock<IBlockchainProcessor>();
            MockProgressService = new Mock<IBlockProgressService>();
            Service = new BlockchainBatchProcessorService(
                MockProcessor.Object, MockProgressService.Object, MaxBlocksPerBatch);
        }

        public class ProcessAsyncTests : BlockchainBatchProcessorServiceTests
        {
            [Fact]
            public async Task When_There_Is_Nothing_To_Process_Returns_Null()
            {
                MockProgressService
                    .Setup(s => s.GetNextBlockRangeToProcessAsync(MaxBlocksPerBatch))
                    .ReturnsAsync((BlockRange?) null);

                var processedRange = await Service.ProcessLatestBlocksAsync();

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

                var processedRange = await Service.ProcessLatestBlocksAsync();

                Assert.Equal(range, processedRange);

                MockProcessor.Verify();
                MockProgressService.Verify();
            }
        }

        public class ProcessContinuallyAsyncTests : BlockchainBatchProcessorServiceTests
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
