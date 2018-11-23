using System;
using System.Threading.Tasks;

namespace Nethereum.BlockchainProcessing.Processing
{
    public interface IBlockProgressRepository
    {
        Task UpsertProgressAsync(ulong blockNumber);
        Task<ulong?> GetLastBlockNumberProcessedAsync();
    }
}