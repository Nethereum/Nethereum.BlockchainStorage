using System.Threading.Tasks;
using Nethereum.RPC.Eth.DTOs;

namespace Nethereum.BlockchainStore.Handlers
{
    public class NullBlockHandler : IBlockHandler
    {
        public Task HandleAsync(Block block)
        {
            return Task.CompletedTask;
        }
    }
}