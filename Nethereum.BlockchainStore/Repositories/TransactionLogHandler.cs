using System.Threading.Tasks;
using Nethereum.BlockchainStore.Handlers;
using Newtonsoft.Json.Linq;

namespace Nethereum.BlockchainStore.Repositories
{
    public class TransactionLogHandler : ITransactionLogHandler
    {
        private readonly ITransactionLogRepository _transactionLogRepository;

        public TransactionLogHandler(ITransactionLogRepository transactionLogRepository)
        {
            this._transactionLogRepository = transactionLogRepository;
        }

        public async Task HandleAsync(string transactionHash, long logIndex, JObject log)
        {
            await _transactionLogRepository.UpsertAsync(transactionHash, logIndex, log);
        }
    }
}
