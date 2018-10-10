using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Nethereum.BlockchainStore.Handlers
{
    public class NullContractHandler : IContractHandler
    {
        private readonly HashSet<string> _cachedContracts = new HashSet<string>(StringComparer.InvariantCultureIgnoreCase);

        public Task<bool> ExistsAsync(string contractAddress)
        {
            return Task.FromResult(_cachedContracts.Contains(contractAddress));
        }

        public Task HandleAsync(ContractTransaction contractTransaction)
        {
            _cachedContracts.Add(contractTransaction.ContractAddress);
            return Task.CompletedTask;
        }
    }
}