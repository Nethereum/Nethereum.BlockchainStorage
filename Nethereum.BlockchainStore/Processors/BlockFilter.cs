using System;
using System.Threading.Tasks;
using Nethereum.RPC.Eth.DTOs;

namespace Nethereum.BlockchainStore.Processors
{
    public class BlockFilter : Filter<BlockWithTransactionHashes>, IBlockFilter
    {
        public BlockFilter(Func<BlockWithTransactionHashes, Task<bool>> condition) : base(condition)
        {
        }
    }
}