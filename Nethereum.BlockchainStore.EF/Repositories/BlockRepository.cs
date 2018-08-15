﻿using System.Data.Entity.Migrations;
using System.Threading.Tasks;
using Nethereum.BlockchainStore.Processors;
using Nethereum.Hex.HexTypes;
using Nethereum.RPC.Eth.DTOs;
using Block = Nethereum.BlockchainStore.Entities.Block;

namespace Nethereum.BlockchainStore.EF.Repositories
{
    public class BlockRepository : RepositoryBase, IBlockRepository
    {
        public BlockRepository(IBlockchainDbContextFactory contextFactory) : base(contextFactory)
        {
        }

        public async Task UpsertBlockAsync(BlockWithTransactionHashes source)
        {
            using (var context = _contextFactory.CreateContext())
            {
                var block = await context.Blocks.FindByBlockNumberAsync(source.Number).ConfigureAwait(false) ?? new Block();

                MapBlock(source, block);

                block.UpdateRowDates();

                context.Blocks.AddOrUpdate(block);

                await context.SaveChangesAsync().ConfigureAwait(false) ;
            }
        }

        public void MapBlock(BlockWithTransactionHashes source, Block block)
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
