using System.Threading.Tasks;

namespace Nethereum.BlockchainProcessing.Handlers
{
    public interface ITransactionLogHandler
    {
        Task HandleAsync(TransactionLogWrapper transactionLog);
    }

    public interface ITransactionLogHandler<TEvent> : ITransactionLogHandler where TEvent : new(){

    }
}
