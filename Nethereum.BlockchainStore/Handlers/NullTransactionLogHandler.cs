using System.Threading.Tasks;

namespace Nethereum.BlockchainStore.Handlers
{
    public class NullTransactionLogHandler : ITransactionLogHandler
    {
        public Task HandleAsync(TransactionLog transactionLog)
        {
            return Task.CompletedTask;
        }
    }
}