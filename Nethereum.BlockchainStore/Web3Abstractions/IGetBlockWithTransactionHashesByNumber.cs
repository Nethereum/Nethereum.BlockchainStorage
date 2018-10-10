using Nethereum.RPC.Eth.DTOs;
using System.Threading.Tasks;

namespace Nethereum.BlockchainStore.Web3Abstractions
{
    public interface IGetBlockWithTransactionHashesByNumber
    {
        Task<BlockWithTransactions> GetBlockWithTransactionsAsync(long blockNumber);
    }
}
