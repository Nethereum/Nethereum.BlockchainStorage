using System.Threading.Tasks;

namespace Nethereum.BlockchainStore.Handlers
{
    public class NullTransactionLogHandler : ITransactionLogHandler
    {
        public Task HandleAsync(TransactionLogWrapper transactionLog)
        {
            return Task.CompletedTask;
        }
    }
}