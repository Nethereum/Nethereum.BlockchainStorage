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

        public async Task HandleAsync(TransactionLog txLog)
        {
            await _transactionLogRepository.UpsertAsync(
                txLog.TransactionHash, txLog.LogIndex, txLog.Log);
        }
    }
}
