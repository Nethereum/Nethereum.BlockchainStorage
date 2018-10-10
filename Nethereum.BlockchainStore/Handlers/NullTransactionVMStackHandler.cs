using System.Threading.Tasks;

namespace Nethereum.BlockchainStore.Handlers
{
    public class NullTransactionVMStackHandler : ITransactionVMStackHandler
    {
        public Task HandleAsync(TransactionVmStack transactionVmStack)
        {
            return Task.CompletedTask;
        }
    }
}