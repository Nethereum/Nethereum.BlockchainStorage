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

        public Task HandleAsync(string transactionHash, string address, JObject stackTrace)
        {
            Log($"[TransactionVmStack] Hash:{transactionHash}, Address:{address}");
            return Task.CompletedTask;
        }
    }
}
