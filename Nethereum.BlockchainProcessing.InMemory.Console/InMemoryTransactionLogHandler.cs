using Nethereum.BlockchainStore.Handlers;
using Newtonsoft.Json.Linq;
using System;
using System.Threading.Tasks;

namespace Nethereum.BlockchainStore.Processing
{
    public class InMemoryTransactionLogHandler : InMemoryHandlerBase, ITransactionLogHandler
    {
        public InMemoryTransactionLogHandler(Action<string> logAction) : base(logAction)
        {
        }

        public Task HandleAsync(TransactionLog transactionLog)
        {
            Log($"[TransactionLog] Hash:{transactionLog.TransactionHash}, Index:{transactionLog.LogIndex}, Address:{transactionLog.Log["address"]}");
            return Task.CompletedTask;
        }
    }
}
