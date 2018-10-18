using System.Threading.Tasks;
using Nethereum.BlockchainProcessing.Handlers;
using Nethereum.RPC.Eth.DTOs;

namespace Nethereum.BlockchainStore.Repositories.Handlers
{
    public class TransactionLogRepositoryHandler : ITransactionLogHandler
    {
        private readonly ITransactionLogRepository _transactionLogRepository;

        public TransactionLogRepositoryHandler(ITransactionLogRepository transactionLogRepository)
        {
            _transactionLogRepository = transactionLogRepository;
        }

        public async Task HandleAsync(TransactionLogWrapper txLog)
        {
            await _transactionLogRepository.UpsertAsync(
                txLog.Log);
        }
    }
}
