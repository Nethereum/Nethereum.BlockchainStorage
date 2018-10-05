using Nethereum.BlockchainStore.Processors;
using System.Threading;
using System.Threading.Tasks;
using Common.Logging;

namespace Nethereum.BlockchainStore.Processing
{
    public class BlockchainProcessor
    {
        private readonly IBlockchainProcessingStrategy _strategy;
        private readonly IBlockProcessor _blockProcessor;
        private readonly ILog _log;

        public BlockchainProcessor(
            IBlockchainProcessingStrategy strategy, 
            IBlockProcessor blockProcessor,
            ILog log = null
            )
        {
            this._strategy = strategy;
            this._blockProcessor = blockProcessor;
            this._log = log;
        }

        /// <summary>
        /// Allow the processor to resume from where it left off
        /// </summary>
        private async Task<long> GetStartingBlockNumber()
        {
            var blockNumber = await _strategy.GetLastBlockProcessedAsync();
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

            await _strategy.FillContractCacheAsync().ConfigureAwait(false);

            return await new BlockEnumeration(
                    (blkNumber) => _blockProcessor.ProcessBlockAsync(blkNumber), 
                    (retryNum) => _strategy.WaitForNextBlock(retryNum),
                    (retryNum) => _strategy.PauseFollowingAnError(retryNum), 
                    _strategy.MaxRetries,
                    cancellationToken,
                    startBlock.Value, 
                    endBlock,
                    _log
                    )
                .ExecuteAsync().ConfigureAwait(false);
        }

    }
}