using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Nethereum.BlockchainProcessing.Handlers;

namespace Nethereum.BlockchainProcessing.InMemory.Console
{
    public class InMemoryContractHandler : InMemoryHandlerBase, IContractHandler
    {
        readonly HashSet<string> _cachedContracts = new HashSet<string>();

        public InMemoryContractHandler(Action<string> logAction) : base(logAction)
        {
        }

        public Task<bool> ExistsAsync(string contractAddress)
        {
            var isCached = _cachedContracts.Contains(contractAddress);
            if (isCached)
            {
                Log($"[Contract Cache Hit] {contractAddress}");
            }

            return Task.FromResult(isCached);
        }

        public Task HandleAsync(ContractTransaction contractTransaction)
        {
            Log($"[Contract Add] Block:{contractTransaction.Transaction.BlockNumber.Value}, Hash:{contractTransaction.Transaction.TransactionHash}, Contract:{contractTransaction.ContractAddress}, Sender:{contractTransaction.Transaction.From}");
            _cachedContracts.Add(contractTransaction.ContractAddress);
            return Task.CompletedTask;
        }
    }
}
