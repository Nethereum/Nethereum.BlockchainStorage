using Nethereum.RPC.Eth.DTOs;
using System.Threading.Tasks;

namespace Nethereum.BlockchainStore.Handlers
{
    public interface IBlockHandler
    {
        Task HandleAsync(BlockWithTransactionHashes block);
    }

    public class NullBlockHandler : IBlockHandler
    {
        public Task HandleAsync(BlockWithTransactionHashes block)
        {
            return Task.CompletedTask;
        }
    }
}
