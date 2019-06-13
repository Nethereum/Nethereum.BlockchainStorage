using System.Numerics;
using System.Threading.Tasks;

namespace Nethereum.BlockchainProcessing.Processors
{
    public interface IBlockProcessor
    {
        bool ProcessTransactionsInParallel { get;set; }
        Task ProcessBlockAsync(BigInteger blockNumber);
        Task<BigInteger> GetMaxBlockNumberAsync();
    }
}