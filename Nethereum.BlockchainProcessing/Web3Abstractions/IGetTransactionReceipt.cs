using System.Threading.Tasks;
using Nethereum.RPC.Eth.DTOs;

namespace Nethereum.BlockchainProcessing.Web3Abstractions
{
    public interface IGetTransactionReceipt
    {
        Task<TransactionReceipt> GetTransactionReceipt(string txnHash);
    }
}