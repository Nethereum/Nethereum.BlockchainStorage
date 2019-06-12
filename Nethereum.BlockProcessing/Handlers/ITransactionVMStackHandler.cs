using System.Threading.Tasks;

namespace Nethereum.BlockchainProcessing.Handlers
{
    public interface ITransactionVMStackHandler
    {
        Task HandleAsync(TransactionVmStack transactionVmStack);
    }
}
