using System.Threading.Tasks;

namespace Nethereum.BlockchainStore.Handlers
{
    public class NullTransactionHandler : ITransactionHandler
    {
        public Task HandleAddressTransactionAsync(AddressTransactionWithReceipt addressTransactionWithReceipt)
        {
            return Task.CompletedTask;
        }

        public Task HandleContractCreationTransactionAsync(ContractCreationTransaction contractCreationTransaction)
        {
            return Task.CompletedTask;
        }

        public Task HandleTransactionAsync(TransactionWithReceipt transactionWithReceipt)
        {
            return Task.CompletedTask;
        }
    }
}