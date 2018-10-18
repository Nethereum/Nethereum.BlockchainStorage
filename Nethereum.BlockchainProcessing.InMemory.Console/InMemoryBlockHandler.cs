using System;
using System.Threading.Tasks;
using Nethereum.BlockchainProcessing.Handlers;
using Nethereum.RPC.Eth.DTOs;

namespace Nethereum.BlockchainProcessing.InMemory.Console
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
