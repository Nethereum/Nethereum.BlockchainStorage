using System;
using System.Threading.Tasks;
using Nethereum.BlockchainProcessing.Handlers;

namespace Nethereum.BlockchainProcessing.InMemory.Console
{
    public class InMemoryTransactionLogHandler : InMemoryHandlerBase, ITransactionLogHandler
    {
        public InMemoryTransactionLogHandler(Action<string> logAction) : base(logAction)
        {
        }

        public Task HandleAsync(TransactionLogWrapper transactionLog)
        {
            Log($"[TransactionLog] Hash:{transactionLog.Transaction.TransactionHash}, Index:{transactionLog.LogIndex}, Address:{transactionLog.Log.Address}");
            return Task.CompletedTask;
        }
    }
}
