using System.Threading.Tasks;
using Nethereum.BlockchainStore.Entities;
using Nethereum.Hex.HexTypes;
using Nethereum.RPC.Eth.DTOs;

namespace Nethereum.BlockchainStore.Repositories
{
    public interface IBlockRepository
    {
        Task UpsertBlockAsync(BlockWithTransactionHashes source);
        Task<long> GetMaxBlockNumberAsync();
        Task<IBlockView> FindByBlockNumberAsync(HexBigInteger blockNumber);
    }
}