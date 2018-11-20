using System.Threading.Tasks;

namespace Nethereum.BlockchainProcessing.Processing
{
    public interface IBlockchainProcessingProgressService
    {
        Task UpsertBlockNumberProcessedTo(ulong blockNumber);
        Task<ulong> GetBlockNumberToProcessFrom();
        Task<ulong> GetBlockNumberToProcessTo();
    }
}
