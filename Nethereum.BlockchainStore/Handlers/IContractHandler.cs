using System.Collections.Generic;
using Nethereum.RPC.Eth.DTOs;
using System.Threading.Tasks;
using System;

namespace Nethereum.BlockchainStore.Handlers
{
    public interface IContractHandler
    {
        Task HandleAsync(string contractAddress, string code, Transaction transaction);
        Task<bool> ExistsAsync(string contractAddress);
    }

    public class NullContractHandler : IContractHandler
    {
        private readonly HashSet<string> _cachedContracts = new HashSet<string>(StringComparer.InvariantCultureIgnoreCase);

        public Task<bool> ExistsAsync(string contractAddress)
        {
            return Task.FromResult(_cachedContracts.Contains(contractAddress));
        }

        public Task HandleAsync(string contractAddress, string code, Transaction transaction)
        {
            _cachedContracts.Add(contractAddress);
            return Task.CompletedTask;
        }
    }
}
