using System.Threading.Tasks;

namespace Nethereum.BlockchainStore.Processing
{
    public interface IWaitStrategy
    {
        Task Apply(int retryCount);
    }
}