using System.Threading.Tasks;

namespace Nethereum.BlockchainProcessing.Handlers
{
    public class NullTransactionVMStackHandler : ITransactionVMStackHandler
    {
        public Task HandleAsync(TransactionVmStack transactionVmStack)
        {
            return Task.CompletedTask;
        }
    }
}