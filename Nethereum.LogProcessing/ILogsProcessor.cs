using Nethereum.BlockchainProcessing.Processing;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Nethereum.LogProcessing
{

    public interface ILogsProcessor : IDisposable
    {
        event EventHandler OnDisposing;

        Task<ulong> ProcessContinuallyAsync(CancellationToken cancellationToken, Action<LogBatchProcessedArgs> batchCompleteCallback = null);
        Task ProcessContinuallyInBackgroundAsync(CancellationToken cancellationToken, Action<LogBatchProcessedArgs> batchCompleteCallback = null, Action<Exception> fatalErrorCallback = null);
        Task<BlockRange?> ProcessOnceAsync(CancellationToken cancellationToken = default);
    }
}