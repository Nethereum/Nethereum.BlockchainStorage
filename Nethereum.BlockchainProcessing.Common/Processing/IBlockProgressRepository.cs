using System;
using System.Numerics;
using System.Threading.Tasks;

namespace Nethereum.BlockchainProcessing.Processing
{
    public interface IBlockProgressRepository
    {
        Task UpsertProgressAsync(BigInteger blockNumber);
        Task<BigInteger?> GetLastBlockNumberProcessedAsync();
    }
}