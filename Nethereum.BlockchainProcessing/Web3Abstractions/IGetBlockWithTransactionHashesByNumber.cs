using System.Threading.Tasks;
using Nethereum.RPC.Eth.DTOs;

namespace Nethereum.BlockchainProcessing.Web3Abstractions
{
    public interface IGetBlockWithTransactionHashesByNumber
    {
        Task<BlockWithTransactions> GetBlockWithTransactionsAsync(ulong blockNumber);
    }
}
