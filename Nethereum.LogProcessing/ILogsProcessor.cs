using Nethereum.BlockchainProcessing.Processing;
using Nethereum.Contracts;
using System;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;

namespace Nethereum.LogProcessing
{

    public interface ILogsProcessor : IDisposable
    {
        event EventHandler OnDisposing;

        Task<BigInteger> ProcessContinuallyAsync(CancellationToken cancellationToken, Action<LogBatchProcessedArgs> batchCompleteCallback = null);
        Task ProcessContinuallyInBackgroundAsync(CancellationToken cancellationToken, Action<LogBatchProcessedArgs> batchCompleteCallback = null, Action<Exception> fatalErrorCallback = null);
        Task<BlockRange?> ProcessOnceAsync(CancellationToken cancellationToken = default);
    }
}