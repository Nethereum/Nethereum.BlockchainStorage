using Nethereum.BlockchainProcessing.Handlers;
using Nethereum.Contracts;
using System.Threading.Tasks;

namespace Nethereum.BlockchainProcessing.InMemory.Console
{
    public class ContractCreationPrinter<TDeploymentMessage>: ITransactionHandler where TDeploymentMessage : ContractDeploymentMessage, new()
    {
        public Task HandleContractCreationTransactionAsync(ContractCreationTransaction tx)
        {
            var dto = tx.DecodeToDeploymentMessage<TDeploymentMessage>();

            if (dto == null) return Task.CompletedTask;

            System.Console.WriteLine($"[CONTRACT CREATION]");
            System.Console.WriteLine($"\t{dto.GetType().Name ?? "unknown"}");
   
            foreach (var prop in dto.GetType().GetProperties())
            {
                System.Console.WriteLine($"\t\t[{prop.Name}:{prop.GetValue(dto) ?? "null"}]");
            }

            return Task.CompletedTask;
        }

        public Task HandleTransactionAsync(TransactionWithReceipt txnWithReceipt)
        {
            return Task.CompletedTask;
        }
    }
}
