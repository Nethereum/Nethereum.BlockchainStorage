using Nethereum.RPC.Eth.DTOs;

namespace Nethereum.BlockchainStore.Processors
{
    public interface IBlockFilter
    {
        bool IsMatch(BlockWithTransactionHashes block);
    }
}
