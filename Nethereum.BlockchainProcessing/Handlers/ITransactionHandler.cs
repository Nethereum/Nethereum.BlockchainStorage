using System.Threading.Tasks;
using Nethereum.Contracts;

namespace Nethereum.BlockchainProcessing.Handlers
{
    public interface ITransactionHandler<TFunctionMessage>: 
        ITransactionHandler where TFunctionMessage : FunctionMessage, new()
    {

    }

    public interface ITransactionHandler
    {
        Task HandleContractCreationTransactionAsync(ContractCreationTransaction contractCreationTransaction);
        Task HandleTransactionAsync(TransactionWithReceipt transactionWithReceipt);
    }
}
