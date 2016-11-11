using System.Threading.Tasks;

namespace Nethereum.BlockchainStore.Processors
{
    public interface IBlockProcessor
    {
        Task ProcessBlockAsync(long blockNumber);
    }
}