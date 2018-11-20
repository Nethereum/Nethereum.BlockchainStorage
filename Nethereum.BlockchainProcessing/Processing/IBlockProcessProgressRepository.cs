using System;
using System.Threading.Tasks;

namespace Nethereum.BlockchainProcessing.Processing
{
    public interface IBlockProcessProgressRepository
    {
        Task UpsertProgressAsync(ulong blockNumber);
        Task<ulong?> GetLatestAsync();
    }
}