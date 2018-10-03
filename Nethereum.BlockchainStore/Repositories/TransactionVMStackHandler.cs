using System.Threading.Tasks;
using Nethereum.BlockchainStore.Handlers;
using Newtonsoft.Json.Linq;

namespace Nethereum.BlockchainStore.Repositories
{
    public class TransactionVMStackHandler : ITransactionVMStackHandler
    {
        private readonly ITransactionVMStackRepository _transactionVmStackRepository;

        public TransactionVMStackHandler(ITransactionVMStackRepository transactionVmStackRepository)
        {
            this._transactionVmStackRepository = transactionVmStackRepository;
        }

        public async Task HandleAsync(string transactionHash, string address, JObject stackTrace)
        {
            await _transactionVmStackRepository.UpsertAsync(transactionHash, address, stackTrace);
        }
    }
}
