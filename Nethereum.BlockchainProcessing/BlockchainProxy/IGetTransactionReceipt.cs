using System.Threading.Tasks;
using Nethereum.RPC.Eth.DTOs;

namespace Nethereum.BlockchainProcessing.BlockchainProxy
{
    public interface IGetTransactionReceipt
    {
        Task<TransactionReceipt> GetTransactionReceipt(string txnHash);
    }
}