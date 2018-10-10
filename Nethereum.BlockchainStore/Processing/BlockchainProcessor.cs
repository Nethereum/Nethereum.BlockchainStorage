using System;
using Microsoft.Extensions.Logging;
using Nethereum.BlockchainStore.Processors;
using System.Threading;
using System.Threading.Tasks;

namespace Nethereum.BlockchainStore.Processing
{
    public class BlockchainProcessor
    {
        private readonly IBlockchainProcessingStrategy _strategy;
        private readonly IBlockProcessor _blockProcessor;
        private readonly ILogger _log = ApplicationLogging.CreateLogger<BlockchainProcessor>();

        public BlockchainProcessor(
            IBlockchainProcessingStrategy strategy, 
            IBlockProcessor blockProcessor
            )
        {
            this._strategy = strategy;
            this._blockProcessor = blockProcessor;
        }

        /// <summary>
        /// Allow the processor to resume from where it left off
        /// </summary>
        private async Task<long> GetStartingBlockNumber()
        {
            _log.LogInformation("Begin GetStartingBlockNumber / _strategy.GetLastBlockProcessedAsync()");
            var blockNumber = await _strategy.GetLastBlockProcessedAsync();

            _log.LogInformation($"GetLastBlockProcessedAsync: {blockNumber}");

            blockNumber = blockNumber <= 0 ? 0 : blockNumber - 1;

            if (_strategy.MinimumBlockNumber > blockNumber)
                return _strategy.MinimumBlockNumber;

            return blockNumber;
        }

        public async Task<bool> ExecuteAsync(
            long? startBlock, long? endBlock)
        {
            return await ExecuteAsync(startBlock, endBlock, new CancellationToken());
        }

        public async Task<bool> ExecuteAsync(
            long? startBlock, long? endBlock, CancellationToken cancellationToken)
        {

            startBlock = startBlock ?? await GetStartingBlockNumber();

            if (endBlock.HasValue && startBlock.Value > endBlock.Value)
                return false;

            _log.LogInformation("Begin FillContractCacheAsync");
            await _strategy.FillContractCacheAsync().ConfigureAwait(false);


            _log.LogInformation("Begin BlockEnumeration");
            return await new BlockEnumeration(
                    (blkNumber) => _blockProcessor.ProcessBlockAsync(blkNumber), 
                    (retryNum) => _strategy.WaitForNextBlock(retryNum),
                    (retryNum) => _strategy.PauseFollowingAnError(retryNum), 
                    () => _blockProcessor.GetMaxBlockNumberAsync(),
                    _strategy.MinimumBlockConfirmations,
                    _strategy.MaxRetries,
                    cancellationToken,
                    startBlock.Value, 
                    endBlock
                    )
                .ExecuteAsync().ConfigureAwait(false);
        }

    }
}