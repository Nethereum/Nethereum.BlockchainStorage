using System;
using Nethereum.RPC.Eth.DTOs;

namespace Nethereum.BlockchainStore.Processors
{
    public class BlockFilter : Filter<BlockWithTransactionHashes>, IBlockFilter
    {
        public BlockFilter(Func<Block, bool> matchFunc) : base(matchFunc)
        {
        }
    }
}