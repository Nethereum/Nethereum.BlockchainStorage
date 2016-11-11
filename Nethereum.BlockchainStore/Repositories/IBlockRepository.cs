using System.Threading.Tasks;
using Nethereum.RPC.Eth.DTOs;

namespace Nethereum.BlockchainStore.Processors
{
    public interface IBlockRepository
    {
        Task UpsertBlockAsync(BlockWithTransactionHashes block);
    }
}