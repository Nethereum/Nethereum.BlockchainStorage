using System.Threading.Tasks;

namespace Nethereum.BlockchainStore.Processors
{
    public interface IBlockProcessor
    {
        bool ProcessTransactionsInParallel { get;set; }
        Task ProcessBlockAsync(long blockNumber);

        Task<long> GetMaxBlockNumberAsync();
    }
}