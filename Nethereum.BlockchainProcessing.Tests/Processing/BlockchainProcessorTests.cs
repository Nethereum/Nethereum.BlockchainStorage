using Moq;
using Nethereum.BlockchainProcessing.Processing;
using Nethereum.BlockchainProcessing.Processors;
using Nethereum.JsonRpc.Client;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Nethereum.BlockchainStore.Tests.Processing
{
    public class BlockchainProcessorTests
    {
        protected BlockchainProcessor Processor;

        protected Mock<IBlockchainProcessingStrategy> MockProcessingStrategy =
            new Mock<IBlockchainProcessingStrategy>();

        protected Mock<IBlockProcessor> MockBlockProcessor = new Mock<IBlockProcessor>();

        public BlockchainProcessorTests()
        {
            Processor = new BlockchainProcessor(MockProcessingStrategy.Object, MockBlockProcessor.Object);
        }

        public class ExecuteAsyncTests : BlockchainProcessorTests
        {

            [Fact]
            public async Task
                When_Start_Block_Is_Higher_Then_End_Returns_False()
            {
                Assert.False(await Processor.ExecuteAsync(startBlock: 1, endBlock: 0));
            }

            [Fact]
            public async Task
                Fills_Contract_Cache_Before_Processing_Blocks()
            {
                var cancellationTokenSource = new CancellationTokenSource();

                MockProcessingStrategy.Setup(s => s.FillContractCacheAsync())
                    .Returns(Task.CompletedTask)
                    .Callback(() => cancellationTokenSource.Cancel());

                await Processor.ExecuteAsync(startBlock: 0, endBlock: 0, cancellationToken: cancellationTokenSource.Token);

                MockProcessingStrategy.VerifyAll();
                MockBlockProcessor.Verify(p => p.ProcessBlockAsync(It.IsAny<long>()), Times.Never);

            }

            public class When_Block_Range_Is_Specified : BlockchainProcessorTests
            {
                [Fact]
                public async Task
                    Will_Process_The_Requested_Block_Range()
                {
                    const long StartBlock = 1000;
                    const long EndBlock = 1205;

                    for (var block = StartBlock; block <= EndBlock; block++)
                    {
                        MockBlockProcessor.Setup(p => p.ProcessBlockAsync(block)).Returns(Task.CompletedTask);
                    }

                    Assert.True(await Processor.ExecuteAsync(StartBlock, EndBlock));

                    MockBlockProcessor.VerifyAll();
                }

                [Fact]
                public async Task Will_Wait_For_Minimum_Block_Confirmations()
                {
                    const int RequiredBlockConfirmations = 6;
                    long maxBlockNumber = 0;
                    int numberOfWaitCycles = 0;
                    int blocksProcessed = 0;

                    MockProcessingStrategy
                        .Setup(s => s.MinimumBlockConfirmations)
                        .Returns(RequiredBlockConfirmations);

                    MockBlockProcessor
                        .Setup(p => p.GetMaxBlockNumberAsync())
                        .Callback(() => maxBlockNumber++)
                        .ReturnsAsync(() => maxBlockNumber);

                    MockProcessingStrategy
                        .Setup(s => s.WaitForNextBlock(It.IsAny<int>()))
                        .Callback<int>((retryNumber) => numberOfWaitCycles++)
                        .Returns(Task.CompletedTask);

                    MockBlockProcessor
                        .Setup(p => p.ProcessBlockAsync(0))
                        .Returns(() =>
                        {
                            blocksProcessed++;
                            return Task.CompletedTask;
                        });

                    var result = await Processor.ExecuteAsync(startBlock: 0, endBlock: 0);

                    Assert.True(result);

                    Assert.Equal(1, blocksProcessed);
                    Assert.Equal(RequiredBlockConfirmations - 1, numberOfWaitCycles);
                    Assert.Equal(RequiredBlockConfirmations, maxBlockNumber);
                    MockBlockProcessor.VerifyAll();
                    MockProcessingStrategy.VerifyAll();
                }


            }

            public class When_Block_Processor_Throws : BlockchainProcessorTests
            {
                [Fact]
                public async Task Will_Retry_Up_To_Retry_Limit_And_Pause_Between_Each_Attempt()
                {
                    const int MaxRetries = 3;
                    const int ExpectedAttempts = MaxRetries + 1;

                    var blockProcessingException = new RpcClientTimeoutException("fake exception");
                    int timesThrown = 0;

                    MockProcessingStrategy
                        .SetupGet(s => s.MaxRetries).Returns(MaxRetries);

                    MockBlockProcessor
                        .Setup(p => p.ProcessBlockAsync(0))
                        .Callback<long>((blkNum) => timesThrown++)
                        .Throws(blockProcessingException);

                    for (var retryNum = 0; retryNum < MaxRetries; retryNum++)
                    {
                        MockProcessingStrategy
                            .Setup(s => s.PauseFollowingAnError(retryNum))
                            .Returns(Task.CompletedTask);
                    }

                    await Processor.ExecuteAsync(0, 0);

                    Assert.Equal(ExpectedAttempts, timesThrown);
                    MockProcessingStrategy.VerifyAll();

                }
            }

            public class When_End_Block_Is_Not_Specified : BlockchainProcessorTests
            {
                [Fact]
                public async Task
                    Will_Run_Continuously()
                {
                    var cancellationTokenSource = new CancellationTokenSource();

                    MockBlockProcessor
                        .Setup(p => p.ProcessBlockAsync(0))
                        .Returns(Task.CompletedTask)
                        .Verifiable("ProcessBlockAsync should have been called for Block 0");

                    MockBlockProcessor
                        .Setup(p => p.ProcessBlockAsync(1))
                        .Callback<long>(blkNum => cancellationTokenSource.Cancel())
                        .Returns(Task.CompletedTask)
                        .Verifiable("ProcessBlockAsync should have been called for Block 1");

                    var result = await Processor.ExecuteAsync(startBlock: 0, endBlock: null, cancellationToken: cancellationTokenSource.Token);

                    Assert.False(result,
                        "Result should be false because execution was cancelled by cancellation token source after block number 1");

                    MockBlockProcessor.VerifyAll();
                }

                [Fact]
                public async Task Will_Wait_For_Minimum_Block_Confirmations()
                {
                    const int RequiredBlockConfirmations = 6;
                    long maxBlockNumber = 0;
                    int numberOfWaitCycles = 0;
                    int blocksProcessed = 0;

                    var cancellationTokenSource = new CancellationTokenSource();

                    MockProcessingStrategy
                        .Setup(s => s.MinimumBlockConfirmations)
                        .Returns(RequiredBlockConfirmations);

                    MockBlockProcessor
                        .Setup(p => p.GetMaxBlockNumberAsync())
                        .Callback(() => maxBlockNumber++)
                        .ReturnsAsync(() => maxBlockNumber);

                    MockProcessingStrategy
                        .Setup(s => s.WaitForNextBlock(It.IsAny<int>()))
                        .Callback<int>((retryNumber) => numberOfWaitCycles++)
                        .Returns(Task.CompletedTask);

                    MockBlockProcessor
                        .Setup(p => p.ProcessBlockAsync(0))
                        .Returns(() =>
                        {
                            blocksProcessed++;
                            cancellationTokenSource.Cancel();
                            return Task.CompletedTask;
                        });

                    var result = await Processor.ExecuteAsync(startBlock: 0, endBlock: null, cancellationToken: cancellationTokenSource.Token);

                    Assert.False(result, "Result should be false because execution should have been stopped by cancellation token");

                    Assert.Equal(1, blocksProcessed);
                    Assert.Equal(RequiredBlockConfirmations - 1, numberOfWaitCycles);
                    Assert.Equal(RequiredBlockConfirmations, maxBlockNumber);
                    MockBlockProcessor.VerifyAll();
                    MockProcessingStrategy.VerifyAll();
                }

                [Fact]
                public async Task When_A_Block_Is_Not_Found_Will_Ask_Strategy_To_Wait_For_Next_Block_Until_Cancelled()
                {
                    var cancellationTokenSource = new CancellationTokenSource();

                    MockBlockProcessor
                        .Setup(p => p.ProcessBlockAsync(0))
                        .Throws(new BlockNotFoundException(0));

                    MockProcessingStrategy.Setup(s => s.WaitForNextBlock(0)).Returns(Task.CompletedTask);
                    MockProcessingStrategy.Setup(s => s.WaitForNextBlock(1)).Returns(Task.CompletedTask);
                    MockProcessingStrategy.Setup(s => s.WaitForNextBlock(2)).Returns(() =>
                   {
                       cancellationTokenSource.Cancel();
                       return Task.CompletedTask;
                   });

                    var result = await Processor.ExecuteAsync(startBlock: 0, endBlock: null, cancellationToken: cancellationTokenSource.Token);

                    Assert.False(result,
                        "Result should be false because execution was cancelled by cancellation token source");

                    MockBlockProcessor.VerifyAll();
                    MockProcessingStrategy.VerifyAll();

                }

            }

           

            public class When_Start_Block_Is_Not_Specified : BlockchainProcessorTests
            {
                [Fact]
                public async Task
                    Requests_Last_Block_Processed_From_Strategy_And_Uses_The_Previous_Block_Number()
                {
                    const long LastBlockProcessed = 11;
                    const long ExpectedStartBlock = LastBlockProcessed - 1;
                    const long EndingBlock = ExpectedStartBlock;

                    MockProcessingStrategy
                        .Setup(s => s.GetLastBlockProcessedAsync())
                        .ReturnsAsync(LastBlockProcessed);

                    MockBlockProcessor
                        .Setup(p => p.ProcessBlockAsync(ExpectedStartBlock))
                        .Returns(Task.CompletedTask);

                    await Processor.ExecuteAsync(startBlock: null, endBlock: EndingBlock);

                    MockProcessingStrategy.VerifyAll();
                    MockBlockProcessor.VerifyAll();
                }

                [Fact]
                public async Task Last_Block_Processsed_Is_Only_Used_If_Greater_Than_Minimum_Block_Number()
                {
                    const long LastBlockProcessed = 11;
                    const long MinimumStartingBlock = 20;
                    const long EndingBlock = MinimumStartingBlock;

                    MockProcessingStrategy.SetupGet(s => s.MinimumBlockNumber).Returns(MinimumStartingBlock);

                    MockProcessingStrategy
                        .Setup(s => s.GetLastBlockProcessedAsync())
                        .ReturnsAsync(LastBlockProcessed);

                    MockBlockProcessor
                        .Setup(p => p.ProcessBlockAsync(MinimumStartingBlock))
                        .Returns(Task.CompletedTask);

                    await Processor.ExecuteAsync(startBlock: null, endBlock: EndingBlock);

                    MockProcessingStrategy.VerifyAll();
                    MockBlockProcessor.VerifyAll();

                }
            }
        }
    }
}
