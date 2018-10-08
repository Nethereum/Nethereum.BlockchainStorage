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

        public Task HandleAsync(string transactionHash, long logIndex, JObject log)
        {
            Log($"[TransactionLog] Hash:{transactionHash}, Index:{logIndex}, Address:{log["address"]}");
            return Task.CompletedTask;
        }
    }
}
