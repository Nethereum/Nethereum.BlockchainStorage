using Nethereum.BlockchainStore.Handlers;
using Newtonsoft.Json.Linq;
using System;
using System.Threading.Tasks;

namespace Nethereum.BlockchainStore.Processing
{
    public class InMemoryTransactionVmStackHandler : InMemoryHandlerBase, ITransactionVMStackHandler
    {
        public InMemoryTransactionVmStackHandler(Action<string> logAction) : base(logAction)
        {
        }

        public Task HandleAsync(TransactionVmStack transactionVmStack)
        {
            Log($"[TransactionVmStack] Hash:{transactionVmStack.TransactionHash}, Address:{transactionVmStack.Address}");
            return Task.CompletedTask;
        }
    }
}
