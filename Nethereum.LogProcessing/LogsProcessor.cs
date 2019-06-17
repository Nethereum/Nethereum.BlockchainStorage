using Common.Logging;
using Nethereum.BlockchainProcessing;
using Nethereum.BlockchainProcessing.Processing;
using Nethereum.Contracts;
using System;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;

namespace Nethereum.LogProcessing
{

    public class LogsProcessor : ILogsProcessor
    {
        private readonly IBlockchainProcessor _processor;
        private readonly IBlockProgressService _progressService;
        private static readonly uint DefaultMaxNumberOfBlocksPerBatch = 100;
        private readonly LogsProcessorLogger _log;

        public IWaitStrategy WaitForBlockStrategy { get; set; } = new WaitStrategy();

        public event EventHandler OnDisposing;

        /// <summary>
        /// When clients return "too many records" error - automatically retry with reduced batch size
        /// </summary>
        public bool EnableAutoBatchResizing { get; set; } = true;
        public uint MaxNumberOfBlocksPerBatch { get; set; }
        public Action<LogBatchProcessedArgs> BatchProcessedCallback { get; private set; }
        public Action<Exception> FatalErrorCallback { get; private set; }

        public LogsProcessor(
            IBlockchainProcessor processor,
            IBlockProgressService progressService,
            uint? maxNumberOfBlocksPerBatch = null,
            Action<LogBatchProcessedArgs> rangesProcessedCallback = null,
            Action<Exception> fatalErrorCallback = null,
            ILog log = null)
        {
            _processor = processor;
            _progressService = progressService;
            _log = new LogsProcessorLogger(log);

            MaxNumberOfBlocksPerBatch = maxNumberOfBlocksPerBatch ?? DefaultMaxNumberOfBlocksPerBatch;
            BatchProcessedCallback = rangesProcessedCallback;
            FatalErrorCallback = fatalErrorCallback;
        }


        /// <summary>
        /// Processes the blocks dictated by the progress service
        /// </summary>
        /// <returns>Returns the BlockRange processed else null if there were no blocks to process</returns>
        public async Task<BlockRange?> ProcessOnceAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                _log.RetrievingBlockNumberRange();

                var nullableRange = await _progressService
                    .GetNextBlockRangeToProcessAsync(MaxNumberOfBlocksPerBatch)
                    .ConfigureAwait(false);

                if (nullableRange == null)
                {
                    _log.NoBlocksToProcess();
                    return null;
                }

                var range = nullableRange.Value;

                _log.ProcessingBlockRange(range);
                await _processor.ProcessAsync(range, cancellationToken)
                    .ConfigureAwait(false);

                _log.UpdatingBlockProgress(range);
                await _progressService.SaveLastBlockProcessedAsync(range.To)
                    .ConfigureAwait(false);

                return range;
            }
            catch (TooManyRecordsException ex)
            {
                _log.TooManyRecords(ex);

                if (MaxNumberOfBlocksPerBatch > 1 && EnableAutoBatchResizing) // try again with a smaller batch size
                {
                    uint newBatchLimit = MaxNumberOfBlocksPerBatch / 2;

                    _log.ChangingBlocksPerBatch(MaxNumberOfBlocksPerBatch, newBatchLimit);

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
        public async Task<BigInteger> ProcessContinuallyAsync(
            CancellationToken cancellationToken,
            Action<LogBatchProcessedArgs> rangesProcessedCallback = null)
        {
            if (rangesProcessedCallback != null)
            {
                BatchProcessedCallback = rangesProcessedCallback;
            }

            uint batchesProcessed = 0;
            uint currentBatchAttemptCount = 0;
            BigInteger totalBlocksProcessed = 0;

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
                    BatchProcessedCallback?.Invoke(new LogBatchProcessedArgs(batchesProcessed, range.Value));
                }
            }

            return totalBlocksProcessed;
        }

        /// <summary>
        /// Runs on a background thread until cancellation or fatal error
        /// </summary>
        public Task ProcessContinuallyInBackgroundAsync(
            CancellationToken cancellationToken,
            Action<LogBatchProcessedArgs> rangesProcessedCallback = null,
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
                                _log.FatalError(baseEx);
                                FatalErrorCallback?.Invoke(baseEx);
                            }
                        },
                        TaskContinuationOptions.OnlyOnFaulted
                    );
            }
            catch (Exception ex)
            {
                _log.FatalError(ex);
                FatalErrorCallback?.Invoke(ex);
                throw;
            }
        }

        bool disposed = false;

        public void Dispose()
        {
            if (!disposed)
            {
                OnDisposing?.Invoke(this, new EventArgs());
                disposed = true;
            }
        }
    }
}
