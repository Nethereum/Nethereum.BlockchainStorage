using System.Threading.Tasks;
using Nethereum.Hex.HexTypes;
using Nethereum.RPC.Eth.DTOs;
using Block = Nethereum.BlockchainStore.Entities.Block;

namespace Nethereum.BlockchainStore.Processors
{
    public interface IBlockRepository
    {
        Task UpsertBlockAsync(BlockWithTransactionHashes source);
        Task<long> GetMaxBlockNumber();
    }

    public interface IEntityBlockRepository: IBlockRepository
    {
        Task<Block> GetBlockAsync(HexBigInteger blockNumber);
    }
}