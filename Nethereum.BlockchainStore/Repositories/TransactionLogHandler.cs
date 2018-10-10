using Nethereum.BlockchainStore.Handlers;
using System.Threading.Tasks;

namespace Nethereum.BlockchainStore.Repositories
{
    public class TransactionLogHandler : ITransactionLogHandler
    {
        private readonly ITransactionLogRepository _transactionLogRepository;

        public TransactionLogHandler(ITransactionLogRepository transactionLogRepository)
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
