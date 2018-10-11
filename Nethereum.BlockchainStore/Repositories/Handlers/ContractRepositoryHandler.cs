using System.Threading.Tasks;
using Nethereum.BlockchainStore.Handlers;

namespace Nethereum.BlockchainStore.Repositories.Handlers
{
    public class ContractRepositoryHandler : IContractHandler
    {
        private readonly IContractRepository _contractRepository;

        public ContractRepositoryHandler(IContractRepository contractRepository)
        {
            _contractRepository = contractRepository;
        }

        public async Task<bool> ExistsAsync(string contractAddress)
        {
            return await _contractRepository.ExistsAsync(contractAddress);
        }

        public async Task HandleAsync(ContractTransaction contractTransaction)
        {
            await _contractRepository.UpsertAsync(contractTransaction.ContractAddress, contractTransaction.Code, contractTransaction.Transaction);
        }
    }
}
