using Nethereum.RPC.Eth.DTOs;
using System.Threading.Tasks;

namespace Nethereum.BlockchainStore.Web3Abstractions
{
    public interface IGetBlockWithTransactionHashesByNumber
    {
        Task<BlockWithTransactionHashes> GetBlockWithTransactionsHashesByNumber(long blockNumber);
    }
}
