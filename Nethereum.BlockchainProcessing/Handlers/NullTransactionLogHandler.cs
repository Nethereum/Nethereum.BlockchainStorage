using System.Threading.Tasks;

namespace Nethereum.BlockchainProcessing.Handlers
{
    public class NullTransactionLogHandler : ITransactionLogHandler
    {
        public Task HandleAsync(TransactionLogWrapper transactionLog)
        {
            return Task.CompletedTask;
        }
    }
}