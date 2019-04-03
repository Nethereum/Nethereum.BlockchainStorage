using System.Threading;
using System.Threading.Tasks;

namespace Nethereum.BlockchainProcessing.Processing
{
    public interface IBlockchainProcessor
    {
        Task ProcessAsync(BlockRange range);
        Task ProcessAsync(BlockRange range, CancellationToken cancellationToken);
    }
}
