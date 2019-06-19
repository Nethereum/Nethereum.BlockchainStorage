using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Moq;
using Nethereum.BlockchainProcessing.Processing;
using Nethereum.Contracts;
using Xunit;

namespace Nethereum.BlockchainProcessing.Common.Tests
{
    public class BlockProgressServiceTests
    {
        protected internal Web3Mock Web3Mock;
        protected internal Mock<IBlockProgressRepository> ProgressRepo;
        protected internal BlockProgressService Service;

        public BlockProgressServiceTests()
        {
            Web3Mock = new Web3Mock();
            ProgressRepo = new Mock<IBlockProgressRepository>();
            Service = new BlockProgressService(Web3Mock.Web3, 10, ProgressRepo.Object);
        }

        public class GetNextBlockRangeTests : BlockProgressServiceTests
        {
            [Fact]
            public async Task When_Nothing_Has_Been_Processed_And_Default_Starting_Block_Is_Null_Returns_Current_Block_On_The_Chain_Less_Min_Confirmations()
            {
                Service = new BlockProgressService(Web3Mock.Web3, null, ProgressRepo.Object, minimumBlockConfirmations: 6);

                Web3Mock.BlockNumberMock.Setup(r => r.SendRequestAsync(null)).ReturnsAsync(20.ToHexBigInteger());
                ProgressRepo.Setup(r => r.GetLastBlockNumberProcessedAsync()).ReturnsAsync((BigInteger?)null);

                var range = await Service.GetNextBlockRangeToProcessAsync(100);

                var expectedBlockRange = new BlockRange(14, 14);
                Assert.Equal(expectedBlockRange, range.Value);
            }


            [Fact]
            public async Task When_Nothing_Has_Been_Processed_Returns_Specified_Starting_Block()
            {
                Web3Mock.BlockNumberMock.Setup(r => r.SendRequestAsync(null)).ReturnsAsync(20.ToHexBigInteger());
                ProgressRepo.Setup(r => r.GetLastBlockNumberProcessedAsync()).ReturnsAsync((ulong?)null);

                var range = await Service.GetNextBlockRangeToProcessAsync(100);

                var expectedBlockRange = new BlockRange(10, 20);
                Assert.Equal(expectedBlockRange, range.Value);
            }

            [Fact]
            public async Task Returns_Next_Unprocessed_Block()
            {
                Web3Mock.BlockNumberMock.Setup(r => r.SendRequestAsync(null)).ReturnsAsync(50.ToHexBigInteger());
                ProgressRepo.Setup(r => r.GetLastBlockNumberProcessedAsync()).ReturnsAsync(5);

                var range = await Service.GetNextBlockRangeToProcessAsync(100);

                var expectedBlockRange = new BlockRange(6, 50);
                Assert.Equal(expectedBlockRange, range.Value);
            }

            /// <summary>
            /// The max block on the chain may have already been processed
            /// The next block to process is last block processed plus one
            /// However there is no use processing if that next block does not yet exist on chain
            /// </summary>
            /// <returns></returns>
            [Fact]
            public async Task When_Last_Block_Processed_Exceeds_Starting_Block_Returns_Null()
            {
                Web3Mock.BlockNumberMock.Setup(r => r.SendRequestAsync(null)).ReturnsAsync(50.ToHexBigInteger());
                ProgressRepo.Setup(r => r.GetLastBlockNumberProcessedAsync()).ReturnsAsync((ulong)50);

                Assert.Null(await Service.GetNextBlockRangeToProcessAsync(100));
            }

        }

        public class SaveLastBlockProcessedTests : BlockProgressServiceTests
        {
            [Fact]
            public async Task Calls_Upsert_On_Repository()
            {
                const ulong LastBlockNumberProcessed = 99;

                await Service.SaveLastBlockProcessedAsync(LastBlockNumberProcessed);

                ProgressRepo.Verify(r => r.UpsertProgressAsync(LastBlockNumberProcessed), Times.Once);
            }
        }

    }
}
