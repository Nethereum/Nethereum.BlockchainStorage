using Nethereum.BlockchainStore.Handlers;
using Nethereum.RPC.Eth.DTOs;
using System;
using System.Threading.Tasks;
using Nethereum.BlockchainStore.Repositories;

namespace Nethereum.BlockchainStore.Processing
{
    public class InMemoryBlockHandler : InMemoryHandlerBase, IBlockHandler
    {
        public InMemoryBlockHandler(Action<string> logAction) : base(logAction)
        {
        }

        public Task HandleAsync(Block block)
        {
            Log($"Block: {block.Number.Value}. Txn Count: {block.TransactionCount()}");
            return Task.CompletedTask;
        }
    }
}
