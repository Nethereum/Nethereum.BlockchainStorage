using System.Threading.Tasks;
using Nethereum.RPC.Eth.DTOs;

namespace Nethereum.BlockchainProcessing.Web3Abstractions
{
    public interface IGetTransactionByHash
    {
        Task<Transaction> GetTransactionByHash(string txnHash);
    }
}
