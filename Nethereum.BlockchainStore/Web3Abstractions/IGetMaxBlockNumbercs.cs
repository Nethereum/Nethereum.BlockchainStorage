using System.Threading.Tasks;

namespace Nethereum.BlockchainStore.Web3Abstractions
{
    public interface IGetMaxBlockNumber
    {
        Task<long> GetMaxBlockNumberAsync();
    }
}
