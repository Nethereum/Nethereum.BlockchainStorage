using System.Threading.Tasks;
using Nethereum.BlockchainStore.Handlers;

namespace Nethereum.BlockchainStore.Repositories.Handlers
{
    public class TransactionVMStackRepositoryHandler : ITransactionVMStackHandler
    {
        private readonly ITransactionVMStackRepository _transactionVmStackRepository;

        public TransactionVMStackRepositoryHandler(ITransactionVMStackRepository transactionVmStackRepository)
        {
            this._transactionVmStackRepository = transactionVmStackRepository;
        }

        public async Task HandleAsync(TransactionVmStack transactionVmStack)
        {
            await _transactionVmStackRepository.UpsertAsync(transactionVmStack.TransactionHash, transactionVmStack.Address, transactionVmStack.StackTrace);
        }
    }
}
