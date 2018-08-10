using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Nethereum.BlockchainStore.Processors;
using Nethereum.Hex.HexTypes;
using Nethereum.RPC.Eth.DTOs;

namespace Nethereum.BlockchainStore.SqlServer.Repositories
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
                var block = await context.Blocks.FindByBlockNumberAsync(source.Number).ConfigureAwait(false) ?? new Entities.Block();

                MapBlock(source, block);

                block.UpdateRowDates();

                if (block.IsNew())
                    context.Blocks.Add(block);
                else
                    context.Blocks.Update(block);

                await context.SaveChangesAsync().ConfigureAwait(false) ;
            }
        }

        public void MapBlock(BlockWithTransactionHashes source, Entities.Block block)
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
