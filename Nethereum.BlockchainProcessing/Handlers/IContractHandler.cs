using System.Threading.Tasks;

namespace Nethereum.BlockchainProcessing.Handlers
{

    public interface IContractHandler
    {
        Task HandleAsync(ContractTransaction contractTransaction);
        Task<bool> ExistsAsync(string contractAddress);
    }
}
