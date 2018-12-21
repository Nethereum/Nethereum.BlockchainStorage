using System.Threading.Tasks;

namespace Nethereum.BlockchainProcessing.BlockchainProxy
{
    public interface IGetMaxBlockNumber
    {
        Task<ulong> GetMaxBlockNumberAsync();
    }
}
