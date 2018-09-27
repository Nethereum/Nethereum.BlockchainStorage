using Nethereum.RPC.Eth.DTOs;
using System.Threading.Tasks;

namespace Nethereum.BlockchainStore.Web3Abstractions
{
    public interface IGetTransactionByHash
    {
        Task<Transaction> GetTransactionByHash(string txnHash);
    }
}
