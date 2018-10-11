using System.Threading.Tasks;
using Nethereum.BlockchainStore.Handlers;

namespace Nethereum.BlockchainStore.Repositories.Handlers
{
    public class TransactionLogRepositoryHandler : ITransactionLogHandler
    {
        private readonly ITransactionLogRepository _transactionLogRepository;

        public TransactionLogRepositoryHandler(ITransactionLogRepository transactionLogRepository)
        {
            _transactionLogRepository = transactionLogRepository;
        }

        public async Task HandleAsync(TransactionLog transactionLog)
        {
            await _transactionLogRepository.UpsertAsync(
                transactionLog.TransactionHash, transactionLog.LogIndex, transactionLog.Log);
        }
    }
}
