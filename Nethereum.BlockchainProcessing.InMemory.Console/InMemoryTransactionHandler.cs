using Nethereum.BlockchainStore.Handlers;
using Nethereum.Hex.HexTypes;
using Nethereum.RPC.Eth.DTOs;
using System;
using System.Threading.Tasks;

namespace Nethereum.BlockchainStore.Processing
{
    public class InMemoryTransactionHandler : InMemoryHandlerBase, ITransactionHandler
    {
        public InMemoryTransactionHandler(Action<string> logAction) : base(logAction)
        {
        }

        public Task HandleAddressTransactionAsync(AddressTransactionWithReceipt addressTransactionWithReceipt)
        {
            Log($"[AddressTransaction] Block:{addressTransactionWithReceipt.Transaction.BlockNumber.Value}, Index:{addressTransactionWithReceipt.Transaction.TransactionIndex.Value}, Hash:{addressTransactionWithReceipt.Transaction.TransactionHash}, Address:{addressTransactionWithReceipt.Address}, From:{addressTransactionWithReceipt.Transaction.From}, To:{addressTransactionWithReceipt.Transaction.To}");
            return Task.CompletedTask;
        }

        public Task HandleContractCreationTransactionAsync(ContractCreationTransaction contractCreationTransaction)
        {
            Log($"[ContractCreation] Block:{contractCreationTransaction.Transaction.BlockNumber.Value}, Index:{contractCreationTransaction.Transaction.TransactionIndex.Value}, Hash:{contractCreationTransaction.Transaction.TransactionHash}, Contract:{contractCreationTransaction.ContractAddress}, From:{contractCreationTransaction.Transaction.From}, To:{contractCreationTransaction.Transaction.To}");
            return Task.CompletedTask;
        }

        public Task HandleTransactionAsync(TransactionWithReceipt transactionWithReceipt)
        {
            Log($"[Transaction] Block:{transactionWithReceipt.Transaction.BlockNumber.Value}, Index:{transactionWithReceipt.Transaction.TransactionIndex.Value}, Hash:{transactionWithReceipt.Transaction.TransactionHash}, From:{transactionWithReceipt.Transaction.From}, To:{transactionWithReceipt.Transaction.To}");
            return Task.CompletedTask;
        }
    }

}
