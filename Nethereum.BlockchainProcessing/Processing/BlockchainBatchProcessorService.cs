using System;
using System.Threading;
using Microsoft.Extensions.Logging;
using Nethereum.Configuration;
using System.Threading.Tasks;

namespace Nethereum.BlockchainProcessing.Processing
{
    public class BlockchainBatchProcessorService
    {
        private readonly IBlockchainProcessor _processor;
        private readonly ILogger _logger = ApplicationLogging.CreateLogger<BlockchainBatchProcessorService>();
        private readonly IBlockchainProcessingProgressService _progressService;
        private readonly uint _maxNumberOfBlocksPerBatch;
        private static readonly uint DefaultMaxNumberOfBlocksPerBatch = 100;

        public IWaitStrategy WaitForBlockStrategy { get; set; } = new WaitStrategy();

        public BlockchainBatchProcessorService(
            IBlockchainProcessor processor, 
            IBlockchainProcessingProgressService  progressService, 
            uint? maxNumberOfBlocksPerBatch = null)
        {
            _processor = processor;
            _progressService = progressService;

            _maxNumberOfBlocksPerBatch = maxNumberOfBlocksPerBatch ?? DefaultMaxNumberOfBlocksPerBatch;
        }

        /// <summary>
        /// Processes the blocks dictated by the progress service
        /// </summary>
        /// <returns>Returns the BlockRange processed else null if there were no blocks to process</returns>
        public async Task<BlockRange?> ProcessLatestBlocksAsync()
        {
            return await ProcessLatestBlocksAsync(new CancellationToken());
        }

        /// <summary>
        /// Processes the blocks dictated by the progress service
        /// </summary>
        /// <returns>Returns the BlockRange processed else null if there were no blocks to process</returns>
        public async Task<BlockRange?> ProcessLatestBlocksAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Getting block number range to process");

            var nullableRange = await _progressService.GetNextBlockRangeToProcess(_maxNumberOfBlocksPerBatch);

            if (nullableRange == null)
            {
                _logger.LogInformation("No block range to process - the most recent block may already have been processed");
                return null;
            }

            var range = nullableRange.Value;

            _logger.LogInformation($"Processing Block Range. from: {range.From} to {range.To}");
            await _processor.ProcessAsync(range, cancellationToken);

            _logger.LogInformation($"Updating current process progress to: {range.To}");
            await _progressService.UpsertBlockNumberProcessedTo(range.To);

            return range;
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
            uint rangesProcessed = 0;
            uint rangeAttemptCount = 0;
            ulong blocksProcessed = 0;

            while (true)
            {
                if (cancellationToken.IsCancellationRequested) break;

                rangeAttemptCount++;
                var range = await ProcessLatestBlocksAsync(cancellationToken);
                
                if (cancellationToken.IsCancellationRequested) break;

                if (range == null) // assume we're up to date - wait for next block
                {
                    await WaitForBlockStrategy.Apply(rangeAttemptCount);
                }
                else // block range was processed so continue straight to the next
                {
                    rangesProcessed++;
                    blocksProcessed += range.Value.BlockCount;
                    rangeAttemptCount = 0;
                    rangesProcessedCallback?.Invoke(rangesProcessed, range.Value);
                }
            }

            return blocksProcessed;
        }

    }
}
