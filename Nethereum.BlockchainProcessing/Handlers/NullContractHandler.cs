using Nethereum.BlockchainProcessing.Nethereum.RPC.Eth.DTOs;
using System.Threading.Tasks;

namespace Nethereum.BlockchainProcessing.Handlers
{
    public class NullContractHandler : IContractHandler
    {
        private readonly UniqueAddressList _cachedContracts = new UniqueAddressList();

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