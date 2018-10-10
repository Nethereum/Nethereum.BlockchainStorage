using System.Threading.Tasks;

namespace Nethereum.BlockchainStore.Handlers
{

    public interface IContractHandler
    {
        Task HandleAsync(ContractTransaction contractTransaction);
        Task<bool> ExistsAsync(string contractAddress);
    }
}
