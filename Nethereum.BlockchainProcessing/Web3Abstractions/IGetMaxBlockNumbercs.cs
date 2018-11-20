using System.Threading.Tasks;

namespace Nethereum.BlockchainProcessing.Web3Abstractions
{
    public interface IGetMaxBlockNumber
    {
        Task<ulong> GetMaxBlockNumberAsync();
    }
}
