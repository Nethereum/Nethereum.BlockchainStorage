using Nethereum.BlockchainStore.Handlers;
using System.Threading.Tasks;

namespace Nethereum.BlockchainProcessing.InMemory.Console
{
    public class FunctionNamePrinter : ITransactionHandler
    {
        private readonly Contracts.Contract _contract;

        public FunctionNamePrinter(Contracts.Contract contract)
        {
            _contract = contract;
        }

        public Task HandleContractCreationTransactionAsync(ContractCreationTransaction contractCreationTransaction)
        {
            return Task.CompletedTask;
        }

        public Task HandleTransactionAsync(TransactionWithReceipt txnWithReceipt)
        {
            var functionDefinition = txnWithReceipt.GetFunctionCaption(_contract);

            System.Console.WriteLine($"[FUNCTION]");
            System.Console.WriteLine($"\t{functionDefinition ?? "unknown"}");

            return Task.CompletedTask;
        }
    }
}
