using System.Threading.Tasks;

namespace Nethereum.BlockchainStore.Handlers
{
    public interface ITransactionLogHandler
    {
        Task HandleAsync(TransactionLogWrapper transactionLog);
    }
}
