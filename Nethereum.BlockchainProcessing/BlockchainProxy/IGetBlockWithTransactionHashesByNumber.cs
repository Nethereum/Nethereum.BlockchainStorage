using System.Threading.Tasks;
using Nethereum.RPC.Eth.DTOs;

namespace Nethereum.BlockchainProcessing.BlockchainProxy
{
    public interface IGetBlockWithTransactionHashesByNumber
    {
        Task<BlockWithTransactions> GetBlockWithTransactionsAsync(ulong blockNumber);
    }
}
