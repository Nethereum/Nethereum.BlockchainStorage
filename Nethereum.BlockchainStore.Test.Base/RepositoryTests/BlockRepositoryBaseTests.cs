using Nethereum.BlockchainStore.Entities.Mapping;
using Nethereum.BlockchainStore.Processors;
using Nethereum.Hex.HexTypes;
using Nethereum.RPC.Eth.DTOs;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Nethereum.BlockchainStore.Test.Base.RepositoryTests
{
    public class BlockRepositoryTests: IRepositoryTest
    {
        private readonly IEntityBlockRepository _repo;

        public BlockRepositoryTests(IEntityBlockRepository repo)
        {
            this._repo = repo;
        }

        public async Task RunAsync()
        {
            await UpsertAsync();
        }

        public async Task UpsertAsync()
        {
            var source = new BlockWithTransactionHashes
            {
                Number = new HexBigInteger(DateTime.Now.Ticks),
                Difficulty = new HexBigInteger("2"),
                GasLimit = new HexBigInteger("4712388"),
                GasUsed = new HexBigInteger("1886574"),
                Size = new HexBigInteger("608"),
                Timestamp = Utils.CreateBlockTimestamp(),
                TotalDifficulty = new HexBigInteger("2027"),
                ExtraData = "0xd983010802846765746887676f312e392e328777696e646f7773000000000000823caa2ecfd32e52827d5fc58e9a6c203c5599e730d0d47c1b10f60ddcff40cb65ef4906023e2e0d32b3814345cce246d3ee56eae47afdb51308b40653940fce01",
                BlockHash = "0x337cd6feedafac6abba40eff40fb1957e08985180f5a03016924ef72fc7b04b9",
                ParentHash = "0x60dd93c2acf312d9379613249d0cdf822a878018ec2d6ecca1e40b9c3ec9cc25",
                Miner = "0xe6de16a66e5cd7270cc36a851818bc092884fe64",
                Nonce = "4",
                TransactionHashes = new []{"0xcb00b69d2594a3583309f332ada97d0df48bae00170e36a4f7bbdad7783fc7e5"}
            };

            await _repo.UpsertBlockAsync(source);

            var storedBlock = await _repo.GetBlockAsync(source.Number);
            Assert.NotNull(storedBlock);

            Assert.Equal(source.Number.Value.ToString(), storedBlock.BlockNumber);
            Assert.Equal(source.Difficulty.ToLong(), storedBlock.Difficulty);
            Assert.Equal(source.GasLimit.ToLong(), storedBlock.GasLimit);
            Assert.Equal(source.GasUsed.ToLong(), storedBlock.GasUsed);
            Assert.Equal(source.Size.ToLong(), storedBlock.Size);
            Assert.Equal(source.Timestamp.ToLong(), storedBlock.Timestamp);
            Assert.Equal(source.TotalDifficulty.ToLong(), storedBlock.TotalDifficulty);
            Assert.Equal(source.ExtraData, storedBlock.ExtraData);
            Assert.Equal(source.BlockHash, storedBlock.Hash);
            Assert.Equal(source.ParentHash, storedBlock.ParentHash);
            Assert.Equal(source.Miner, storedBlock.Miner);
            Assert.Equal(new HexBigInteger(source.Nonce).ToLong(), storedBlock.Nonce);
            Assert.Equal(source.TransactionHashes.Length, storedBlock.TransactionCount);
        }
    }
}
