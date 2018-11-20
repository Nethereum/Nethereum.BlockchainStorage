using System;
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
        private readonly ulong _maxNumberOfBlocksPerBatch;
        private static readonly ulong DefaultMaxNumberOfBlocksPerBatch = 100;

        public BlockchainBatchProcessorService(
            IBlockchainProcessor processor, 
            IBlockchainProcessingProgressService  progressService, 
            ulong? maxNumberOfBlocksPerBatch = null)
        {
            _processor = processor;
            _progressService = progressService;

            _maxNumberOfBlocksPerBatch = maxNumberOfBlocksPerBatch ?? DefaultMaxNumberOfBlocksPerBatch;
        }

        public async Task ProcessLatestBlocks()
        {
            _logger.LogInformation("Getting block number range to process");

            var range = await _progressService.GetBlockRangeToProcess(_maxNumberOfBlocksPerBatch);
                
            _logger.LogInformation($"Getting all data changes events from: {range.from} to {range.to}");
            await _processor.ProcessAsync(range.from, range.to);

            _logger.LogInformation($"Updating current process progress to: {range.to}");
            await _progressService.UpsertBlockNumberProcessedTo(range.to);
        }

    }
}
