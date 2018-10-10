using System;
using System.Threading.Tasks;
using Nethereum.RPC.Eth.DTOs;

namespace Nethereum.BlockchainStore.Processors
{
    public class BlockFilter : Filter<Block>, IBlockFilter
    {
        public BlockFilter(Func<Block, Task<bool>> condition) 
            : base(condition)
        {
        }

        public BlockFilter(Func<Block, bool> condition) : base(condition)
        {
        }
    }
}