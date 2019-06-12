using System;
using System.Threading;
using Microsoft.Extensions.Logging;
using Nethereum.Configuration;
using System.Threading.Tasks;
using Nethereum.BlockchainProcessing.BlockchainProxy;

namespace Nethereum.BlockchainProcessing.Processing.Logs
{
    public class LogsProcessor : ILogsProcessor
    {
        private readonly IBlockchainProcessor _processor;
        private readonly ILogger _logger = ApplicationLogging.CreateLogger<LogsProcessor>();
        private readonly IBlockProgressService _progressService;
        private static readonly uint DefaultMaxNumberOfBlocksPerBatch = 100;

        public IWaitStrategy WaitForBlockStrategy { get; set; } = new WaitStrategy();

        public event EventHandler OnDisposing;

        /// <summary>
        /// When clients return "too many records" error - automatically retry with reduced batch size
        /// </summary>
        public bool EnableAutoBatchResizing { get; set; } = true;
        public uint MaxNumberOfBlocksPerBatch { get; set; }
        public Action<uint, BlockRange> BatchProcessedCallback { get; private set; }
        public Action<Exception> FatalErrorCallback { get; private set; }

        public LogsProcessor(
            IBlockchainProcessor processor,
            IBlockProgressService progressService,
            uint? maxNumberOfBlocksPerBatch = null,
            Action<uint, BlockRange> rangesProcessedCallback = null,
            Action<Exception> fatalErrorCallback = null)
        {
            _processor = processor;
            _progressService = progressService;

            MaxNumberOfBlocksPerBatch = maxNumberOfBlocksPerBatch ?? DefaultMaxNumberOfBlocksPerBatch;
            BatchProcessedCallback = rangesProcessedCallback;
            FatalErrorCallback = fatalErrorCallback;
        }


        /// <summary>
        /// Processes the blocks dictated by the progress service
        /// </summary>
        /// <returns>Returns the BlockRange processed else null if there were no blocks to process</returns>
        public async Task<BlockRange?> ProcessOnceAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                _logger.LogInformation("Getting block number range to process");

                var nullableRange = await _progressService
                    .GetNextBlockRangeToProcessAsync(MaxNumberOfBlocksPerBatch)
                    .ConfigureAwait(false);

                if (nullableRange == null)
                {
                    _logger.LogInformation("No block range to process - the most recent block may already have been processed");
                    return null;
                }

                var range = nullableRange.Value;

                _logger.LogInformation($"Processing Block Range. from: {range.From} to {range.To}");
                await _processor.ProcessAsync(range, cancellationToken)
                    .ConfigureAwait(false);

                _logger.LogInformation($"Updating current process progress to: {range.To}");
                await _progressService.SaveLastBlockProcessedAsync(range.To)
                    .ConfigureAwait(false);

                return range;
            }
            catch (TooManyRecordsException ex)
            {
                _logger.LogWarning($"Too many results error. : {ex.Message}");

                if (MaxNumberOfBlocksPerBatch > 1 && EnableAutoBatchResizing) // try again with a smaller batch size
                {
                    uint newBatchLimit = MaxNumberOfBlocksPerBatch / 2;

                    _logger.LogWarning($"Resetting _maxNumberOfBlocksPerBatch. Old Value:{MaxNumberOfBlocksPerBatch}, New Value: {newBatchLimit}");

                    MaxNumberOfBlocksPerBatch = newBatchLimit;

                    return await ProcessOnceAsync(cancellationToken).ConfigureAwait(false);
                }

                throw;
            }
        }

        /// <summary>
        /// Processes block ranges until cancellation is requested 
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <param name="rangesProcessedCallback"></param>
        /// <returns>The total number of blocks processed</returns>
        public async Task<ulong> ProcessContinuallyAsync(
            CancellationToken cancellationToken,
            Action<uint, BlockRange> rangesProcessedCallback = null)
        {
            if (rangesProcessedCallback != null)
            {
                BatchProcessedCallback = rangesProcessedCallback;
            }

            uint batchesProcessed = 0;
            uint currentBatchAttemptCount = 0;
            ulong totalBlocksProcessed = 0;

            while (true)
            {
                if (cancellationToken.IsCancellationRequested) break;

                currentBatchAttemptCount++;
                var range = await ProcessOnceAsync(cancellationToken)
                    .ConfigureAwait(false);

                if (cancellationToken.IsCancellationRequested) break;

                if (range == null) // assume we're up to date - wait for next block
                {
                    await WaitForBlockStrategy.Apply(currentBatchAttemptCount)
                        .ConfigureAwait(false);
                }
                else // block range was processed so continue straight to the next
                {
                    batchesProcessed++;
                    totalBlocksProcessed += range.Value.BlockCount;
                    currentBatchAttemptCount = 0;
                    BatchProcessedCallback?.Invoke(batchesProcessed, range.Value);
                }
            }

            return totalBlocksProcessed;
        }

        /// <summary>
        /// Runs on a background thread until cancellation or fatal error
        /// </summary>
        public Task ProcessContinuallyInBackgroundAsync(
            CancellationToken cancellationToken,
            Action<uint, BlockRange> rangesProcessedCallback = null,
            Action<Exception> fatalErrorCallback = null)
        {
            try
            {
                if (rangesProcessedCallback != null)
                {
                    BatchProcessedCallback = rangesProcessedCallback;
                }

                if (fatalErrorCallback != null)
                {
                    FatalErrorCallback = fatalErrorCallback;
                }

                return Task.Factory
                    .StartNew
                    (
                        async () => await ProcessContinuallyAsync(cancellationToken, BatchProcessedCallback),
                        cancellationToken,
                        TaskCreationOptions.LongRunning,
                        TaskScheduler.Default
                    )
                    .Unwrap()
                    .ContinueWith
                    (
                        (t) =>
                        {
                            if (t.IsFaulted)
                            {
                                var baseEx = t.Exception.GetBaseException();
                                _logger.LogError(baseEx, baseEx.Message);
                                FatalErrorCallback?.Invoke(baseEx);
                            }
                        },
                        TaskContinuationOptions.OnlyOnFaulted
                    );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{nameof(LogsProcessor)}.ProcessContinuallyInBackgroundAsync threw an initialisation error");
                FatalErrorCallback?.Invoke(ex);
                throw;
            }
        }

        bool disposed = false;

        public void Dispose()
        {
            if(!disposed)
            { 
                OnDisposing?.Invoke(this, new EventArgs());
                disposed = true;
            }
        }
    }
}
