using Nethereum.RPC.Eth.DTOs;
using System.Threading.Tasks;

namespace Nethereum.BlockchainStore.Handlers
{
    public interface IBlockHandler
    {
        Task HandleAsync(Block block);
    }
}
