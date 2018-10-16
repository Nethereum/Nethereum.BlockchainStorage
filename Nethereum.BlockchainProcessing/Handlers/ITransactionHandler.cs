using System.Threading.Tasks;

namespace Nethereum.BlockchainProcessing.Handlers
{

    public interface ITransactionHandler
    {
        Task HandleContractCreationTransactionAsync(ContractCreationTransaction contractCreationTransaction);
        Task HandleTransactionAsync(TransactionWithReceipt transactionWithReceipt);
    }
}
