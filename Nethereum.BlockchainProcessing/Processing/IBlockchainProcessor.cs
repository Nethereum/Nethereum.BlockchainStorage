using System.Threading;
using System.Threading.Tasks;

namespace Nethereum.BlockchainProcessing.Processing
{
    public interface IBlockchainProcessor
    {
        Task ProcessAsync(ulong fromBlockNumber, ulong toBlockNumber);
        Task ProcessAsync(ulong fromBlockNumber, ulong toBlockNumber, CancellationToken cancellationToken);
    }
}
