using System.Threading.Tasks;

namespace Nethereum.BlockchainProcessing.Processors
{
    public interface IBlockProcessor
    {
        bool ProcessTransactionsInParallel { get;set; }
        Task ProcessBlockAsync(ulong blockNumber);
        Task<ulong> GetMaxBlockNumberAsync();
    }
}