using System.Threading.Tasks;
using Nethereum.RPC.Eth.DTOs;

namespace Nethereum.BlockchainProcessing.BlockchainProxy
{
    public interface IGetTransactionByHash
    {
        Task<Transaction> GetTransactionByHash(string txnHash);
    }
}
