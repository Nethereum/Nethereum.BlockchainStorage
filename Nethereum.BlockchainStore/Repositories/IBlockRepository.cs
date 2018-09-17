using System.Threading.Tasks;
using Nethereum.BlockchainStore.Entities;
using Nethereum.Hex.HexTypes;
using Nethereum.RPC.Eth.DTOs;
using Block = Nethereum.BlockchainStore.Entities.Block;

namespace Nethereum.BlockchainStore.Processors
{
    public interface IBlockRepository
    {
        Task UpsertBlockAsync(BlockWithTransactionHashes source);
        Task<long> GetMaxBlockNumber();
        Task<IBlockView> GetBlockAsync(HexBigInteger blockNumber);
    }
}