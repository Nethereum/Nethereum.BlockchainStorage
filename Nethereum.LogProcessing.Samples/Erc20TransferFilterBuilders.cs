using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.BlockchainProcessing.Processing;
using Nethereum.BlockchainProcessing.Processing.Logs;
using Nethereum.Contracts;
using System.Collections.Generic;
using System.Numerics;
using System.Threading.Tasks;
using Xunit;

namespace Nethereum.LogProcessing.Samples
{
    public class Erc20TransferFilterBuilders
    {
        [Event("Transfer")]
        public class TransferEvent_ERC20
        {
            [Parameter("address", "_from", 1, true)]
            public string From { get; set; }

            [Parameter("address", "_to", 2, true)]
            public string To { get; set; }

            [Parameter("uint256", "_value", 3, false)]
            public BigInteger Value { get; set; }
        }

        [Fact]
        public async Task TransfersInBlock()
        {
            var filter = new FilterInputBuilder<TransferEvent_ERC20>()
                .Build(blockRange: new BlockRange(3860820, 3860820));

            var expectedLogs = new List<(string txHash, int logIndex)>
            {
                {("0x99d3d6a1fe9eb4a44ff412bd02f3816b0b9061a2356b52a0f9bbc4b459d3f55a", 0)},
                {("0x70e78bcb16275e38dfd1048625b7a5cb22e21027955d8de301b9fbd411c692d6", 7)},
                {("0x70e78bcb16275e38dfd1048625b7a5cb22e21027955d8de301b9fbd411c692d6", 9)},
                {("0xa4dec5a09100fa98940175c8da804ea53ad3e2bb0022c599d1cecd2ec80d04f7", 25)}
            };

            await Verify(filter, expectedLogs);

        }

        [Fact]
        public async Task TransfersForContract()
        {
            var filter = new FilterInputBuilder<TransferEvent_ERC20>()
                .Build(contractAddress: "0x3678FbEFC663FC28336b93A1FA397B67ae42114d",
                    blockRange: new BlockRange(3860820, 3860820));

            var expectedLogs = new List<(string txHash, int logIndex)>
            {
                {("0xa4dec5a09100fa98940175c8da804ea53ad3e2bb0022c599d1cecd2ec80d04f7", 25)}
            };

            await Verify(filter, expectedLogs);
        }

        [Fact]
        public async Task TransfersFrom()
        {
            var filter = new FilterInputBuilder<TransferEvent_ERC20>()
                .AddTopic(tfr => tfr.From, "0x8c0b71d3d3cf9e3a1cd4854ff5555ad43645b44d")
                .Build(blockRange: new BlockRange(3860820, 3860820));

            var expectedLogs = new List<(string txHash, int logIndex)>
            {
                {("0x99d3d6a1fe9eb4a44ff412bd02f3816b0b9061a2356b52a0f9bbc4b459d3f55a", 0)}
            };

            await Verify(filter, expectedLogs);
        }

        [Fact]
        public async Task TransfersTo()
        {
            var filter = new FilterInputBuilder<TransferEvent_ERC20>()
                .AddTopic(tfr => tfr.To, "0xdfa70b70b41d77a7cdd8b878f57521d47c064d8c")
                .Build(blockRange: new BlockRange(3860820, 3860820));

            var expectedLogs = new List<(string txHash, int logIndex)>
            {
                {("0xa4dec5a09100fa98940175c8da804ea53ad3e2bb0022c599d1cecd2ec80d04f7", 25)}
            };

            await Verify(filter, expectedLogs);
        }

        [Fact]
        public async Task TransfersForContractTo()
        {
            var filter = new FilterInputBuilder<TransferEvent_ERC20>()
                .AddTopic(tfr => tfr.To, "0xdfa70b70b41d77a7cdd8b878f57521d47c064d8c")
                .Build(contractAddress: "0x3678FbEFC663FC28336b93A1FA397B67ae42114d", blockRange: new BlockRange(3860820, 3860820));

            var expectedLogs = new List<(string txHash, int logIndex)>
            {
                {("0xa4dec5a09100fa98940175c8da804ea53ad3e2bb0022c599d1cecd2ec80d04f7", 25)}
            };

            await Verify(filter, expectedLogs);
        }

        [Fact]
        public async Task TransfersForContractFrom()
        {
            var filter = new FilterInputBuilder<TransferEvent_ERC20>()
                .AddTopic(tfr => tfr.From, "0x8c0b71d3d3cf9e3a1cd4854ff5555ad43645b44d")
                .Build(contractAddress: "0x7122fea3a32276e6057ece3cb8bed764189b5e95", blockRange: new BlockRange(3860820, 3860820));


            var expectedLogs = new List<(string txHash, int logIndex)>
            {
                {("0x99d3d6a1fe9eb4a44ff412bd02f3816b0b9061a2356b52a0f9bbc4b459d3f55a", 0)}
            };

            await Verify(filter, expectedLogs);
        }

        [Fact]
        public async Task TransfersForContractFromAndTo()
        {
            var filter = new FilterInputBuilder<TransferEvent_ERC20>()
                .AddTopic(tfr => tfr.From, "0x0000000000000000000000000000000000000000")
                .AddTopic(tfr => tfr.To, "0xdfa70b70b41d77a7cdd8b878f57521d47c064d8c")
                .Build(contractAddress: "0x3678fbefc663fc28336b93a1fa397b67ae42114d", blockRange: new BlockRange(3860820, 3860820));

            var expectedLogs = new List<(string txHash, int logIndex)>
            {
                {("0xa4dec5a09100fa98940175c8da804ea53ad3e2bb0022c599d1cecd2ec80d04f7", 25)}
            };

            await Verify(filter, expectedLogs);
        }

        private static async Task Verify(RPC.Eth.DTOs.NewFilterInput filter, List<(string, int)> expectedLogs)
        {
            var proxy = new Web3.Web3(TestConfiguration.BlockchainUrls.Infura.Rinkeby);
            var logs = await proxy.Eth.Filters.GetLogs.SendRequestAsync(filter);

            Assert.NotEmpty(logs);
            Assert.Equal(expectedLogs.Count, logs.Length);

            for (int i = 0; i < expectedLogs.Count; i++)
            {
                var expectedTxHash = expectedLogs[i].Item1;
                var expectedLogIndex = expectedLogs[i].Item2;
                var transfer = logs[i].DecodeEvent<TransferEvent_ERC20>();

                Assert.Equal(expectedTxHash, transfer.Log.TransactionHash);
                Assert.Equal(expectedLogIndex, (int)transfer.Log.LogIndex.Value);
            }
        }
    }
}
