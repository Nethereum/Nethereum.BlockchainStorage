using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using CsvHelper.Configuration;
using Nethereum.BlockchainStore.Entities;
using Nethereum.BlockchainStore.Entities.Mapping;
using Nethereum.BlockchainStore.Processors;
using Nethereum.BlockchainStore.Repositories;
using Nethereum.Hex.HexTypes;
using Nethereum.RPC.Eth.DTOs;
using Block = Nethereum.BlockchainStore.Entities.Block;

namespace Nethereum.BlockchainStore.Csv.Repositories
{
    public class BlockRepository : CsvRepositoryBase<Block>, IBlockRepository
    {
        public BlockRepository(string csvFolderpath) : base(csvFolderpath, "Blocks")
        {
        }

        public async Task<IBlockView> FindByBlockNumberAsync(HexBigInteger blockNumber)
        {
            return await FindAsync(b => b.BlockNumber == blockNumber.Value.ToString());
        }

        public async Task<long> GetMaxBlockNumberAsync()
        {
            long maxBlock = 0;
            await EnumerateAsync(e =>
            {
                var blockNumber = long.Parse(e.BlockNumber);

                if (maxBlock < blockNumber)
                {
                    maxBlock = blockNumber;
                }
            });

            return maxBlock;
        }

        public async Task UpsertBlockAsync(Nethereum.RPC.Eth.DTOs.Block source)
        {
            var block = new Block();
            block.Map(source);
            block.UpdateRowDates();
            await Write(block).ConfigureAwait(false);
        }

        protected override ClassMap<Block> CreateClassMap()
        {
            return BlockMap.Instance;
        }
    }

    public class BlockMap : ClassMap<Block>
    {
        public static BlockMap Instance = new BlockMap();

        public BlockMap()
        {
            AutoMap();
            Map(b => b.Timestamp).Name("_Timestamp");
        }
    }
}
