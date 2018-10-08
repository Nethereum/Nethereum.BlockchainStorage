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

        public Task HandleAddressTransactionAsync(Transaction transaction, TransactionReceipt transactionReceipt, bool hasError, HexBigInteger blockTimestamp, string address, string error = null, bool hasVmStack = false)
        {
            Log($"[AddressTransaction] Block:{transaction.BlockNumber.Value}, Index:{transaction.TransactionIndex.Value}, Hash:{transaction.TransactionHash}, Address:{address}, From:{transaction.From}, To:{transaction.To}");
            return Task.CompletedTask;
        }

        public Task HandleContractCreationTransactionAsync(string contractAddress, string code, Transaction transaction, TransactionReceipt transactionReceipt, bool failedCreatingContract, HexBigInteger blockTimestamp)
        {
            Log($"[ContractCreation] Block:{transaction.BlockNumber.Value}, Index:{transaction.TransactionIndex.Value}, Hash:{transaction.TransactionHash}, Contract:{contractAddress}, From:{transaction.From}, To:{transaction.To}");
            return Task.CompletedTask;
        }

        public Task HandleTransactionAsync(Transaction transaction, TransactionReceipt transactionReceipt, bool hasError, HexBigInteger blockTimestamp, string error = null, bool hasVmStack = false)
        {
            Log($"[Transaction] Block:{transaction.BlockNumber.Value}, Index:{transaction.TransactionIndex.Value}, Hash:{transaction.TransactionHash}, From:{transaction.From}, To:{transaction.To}");
            return Task.CompletedTask;
        }
    }

}
