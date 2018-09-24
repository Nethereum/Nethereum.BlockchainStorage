using Nethereum.BlockchainStore.Entities;
using Nethereum.Hex.HexTypes;
using Nethereum.RPC.Eth.DTOs;
using System.Threading.Tasks;

namespace Nethereum.BlockchainStore.Processors
{
    public interface IBlockRepository
    {
        Task UpsertBlockAsync(BlockWithTransactionHashes source);
        Task<long> GetMaxBlockNumber();
        Task<IBlockView> FindByBlockNumberAsync(HexBigInteger blockNumber);
    }
}