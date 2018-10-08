using Nethereum.BlockchainStore.Handlers;
using Nethereum.RPC.Eth.DTOs;
using System;
using System.Threading.Tasks;

namespace Nethereum.BlockchainStore.Processing
{
    public class InMemoryBlockHandler : InMemoryHandlerBase, IBlockHandler
    {
        public InMemoryBlockHandler(Action<string> logAction) : base(logAction)
        {
        }

        public Task HandleAsync(BlockWithTransactionHashes block)
        {
            Log($"Block: {block.Number.Value}");
            return Task.CompletedTask;
        }
    }
}
