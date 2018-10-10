using System.Threading.Tasks;

namespace Nethereum.BlockchainStore.Handlers
{

    public interface ITransactionHandler
    {
        Task HandleContractCreationTransactionAsync(ContractCreationTransaction contractCreationTransaction);

        Task HandleTransactionAsync(TransactionWithReceipt transactionWithReceipt);

        Task HandleAddressTransactionAsync(AddressTransactionWithReceipt addressTransactionWithReceipt);

    }
}
