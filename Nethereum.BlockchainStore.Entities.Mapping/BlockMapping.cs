using System;
using Nethereum.Hex.HexTypes;
using Nethereum.RPC.Eth.DTOs;

namespace Nethereum.BlockchainStore.Entities.Mapping
{
    public static class BlockMapping
    {
        public static void Map(this Block block, BlockWithTransactionHashes source)
        {
            block.BlockNumber = source.Number.Value.ToString();
            block.Difficulty = source.Difficulty.ToLong();
            block.GasLimit = source.GasLimit.ToLong();
            block.GasUsed = source.GasUsed.ToLong();
            block.Size = source.Size.ToLong();
            block.Timestamp = source.Timestamp.ToLong();
            block.TotalDifficulty = source.TotalDifficulty.ToLong();
            block.ExtraData = source.ExtraData ?? string.Empty;
            block.Hash = source.BlockHash ?? string.Empty;
            block.ParentHash = source.ParentHash ?? string.Empty;
            block.Miner = source.Miner ?? string.Empty;
            block.Nonce = string.IsNullOrEmpty(source.Nonce) ? 0 : new HexBigInteger(source.Nonce).ToLong();
            block.TransactionCount = source.TransactionHashes.Length;
        }
    }
}
