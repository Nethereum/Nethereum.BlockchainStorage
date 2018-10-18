using System;
using System.Threading.Tasks;
using Nethereum.BlockchainProcessing.Handlers;

namespace Nethereum.BlockchainProcessing.InMemory.Console
{
    public class InMemoryTransactionHandler : InMemoryHandlerBase, ITransactionHandler
    {
        public InMemoryTransactionHandler(Action<string> logAction) : base(logAction)
        {
        }

        public Task HandleContractCreationTransactionAsync(ContractCreationTransaction contractCreationTransaction)
        {
            var addresses = string.Join(",", contractCreationTransaction.GetAllRelatedAddresses());
            Log($"[ContractCreation] Block:{contractCreationTransaction.Transaction.BlockNumber.Value}, Index:{contractCreationTransaction.Transaction.TransactionIndex.Value}, Hash:{contractCreationTransaction.Transaction.TransactionHash}, Contract:{contractCreationTransaction.ContractAddress}, From:{contractCreationTransaction.Transaction.From}, To:{contractCreationTransaction.Transaction.To}, All Addresses: {addresses}");
            return Task.CompletedTask;
        }

        public Task HandleTransactionAsync(TransactionWithReceipt transactionWithReceipt)
        {
            var addresses = string.Join(",", transactionWithReceipt.GetAllRelatedAddresses());
            Log($"[Transaction] Block:{transactionWithReceipt.Transaction.BlockNumber.Value}, Index:{transactionWithReceipt.Transaction.TransactionIndex.Value}, Hash:{transactionWithReceipt.Transaction.TransactionHash}, From:{transactionWithReceipt.Transaction.From}, To:{transactionWithReceipt.Transaction.To}, All Addresses: {addresses}");
            return Task.CompletedTask;
        }
    }

}
