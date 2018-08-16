using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Nethereum.BlockchainStore.EF.Repositories;
using Nethereum.BlockchainStore.Entities.Mapping;
using Nethereum.Hex.HexTypes;
using Nethereum.RPC.Eth.DTOs;
using Utils = Nethereum.BlockchainStore.EF.Tests.Base.Common.Utils;

namespace Nethereum.BlockchainStore.EF.Tests.Base.RepositoryTests
{
    public abstract class BlockRepositoryBaseTests: RepositoryTestBase
    {
        protected BlockRepositoryBaseTests(IBlockchainDbContextFactory contextFactory) : base(contextFactory)
        {
        }

        [TestMethod]
        public async Task UpsertAsync()
        {
            var repo = new BlockRepository(contextFactory);
            //setup

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

            await repo.UpsertBlockAsync(source);

            var context = contextFactory.CreateContext();
            var storedBlock = await context.Blocks.FindByBlockNumberAsync(source.Number);
            Assert.IsNotNull(storedBlock);

            Assert.AreEqual(source.Number.Value.ToString(), storedBlock.BlockNumber);
            Assert.AreEqual(source.Difficulty.ToLong(), storedBlock.Difficulty);
            Assert.AreEqual(source.GasLimit.ToLong(), storedBlock.GasLimit);
            Assert.AreEqual(source.GasUsed.ToLong(), storedBlock.GasUsed);
            Assert.AreEqual(source.Size.ToLong(), storedBlock.Size);
            Assert.AreEqual(source.Timestamp.ToLong(), storedBlock.Timestamp);
            Assert.AreEqual(source.TotalDifficulty.ToLong(), storedBlock.TotalDifficulty);
            Assert.AreEqual(source.ExtraData, storedBlock.ExtraData);
            Assert.AreEqual(source.BlockHash, storedBlock.Hash);
            Assert.AreEqual(source.ParentHash, storedBlock.ParentHash);
            Assert.AreEqual(source.Miner, storedBlock.Miner);
            Assert.AreEqual(new HexBigInteger(source.Nonce).ToLong(), storedBlock.Nonce);
            Assert.AreEqual(source.TransactionHashes.Length, storedBlock.TransactionCount);
        }
    }
}
