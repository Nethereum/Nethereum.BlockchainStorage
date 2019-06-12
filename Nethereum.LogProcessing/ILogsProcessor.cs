using System;
using System.Threading;
using System.Threading.Tasks;

namespace Nethereum.BlockchainProcessing.Processing.Logs
{
    public interface ILogsProcessor: IDisposable
    {
        event EventHandler OnDisposing;

        Task<ulong> ProcessContinuallyAsync(CancellationToken cancellationToken, Action<uint, BlockRange> rangesProcessedCallback = null);
        Task ProcessContinuallyInBackgroundAsync(CancellationToken cancellationToken, Action<uint, BlockRange> rangesProcessedCallback = null, Action<Exception> fatalErrorCallback = null);
        Task<BlockRange?> ProcessOnceAsync(CancellationToken cancellationToken = default);
    }
}