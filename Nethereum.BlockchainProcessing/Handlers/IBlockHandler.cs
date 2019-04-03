using System.Threading.Tasks;
using Nethereum.RPC.Eth.DTOs;

namespace Nethereum.BlockchainProcessing.Handlers
{
    public interface IBlockHandler
    {
        Task HandleAsync(Block block);
    }
}
