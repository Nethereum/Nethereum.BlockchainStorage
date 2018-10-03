using Nethereum.RPC.Eth.DTOs;
using System.Threading.Tasks;

namespace Nethereum.BlockchainStore.Handlers
{
    public interface IContractHandler
    {
        Task HandleAsync(string contractAddress, string code, Transaction transaction);
        Task<bool> ExistsAsync(string contractAddress);
    }
}
