using Common.Logging;
using Nethereum.Contracts;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;

namespace Nethereum.BlockchainProcessing.Processing
{

    public class BlockchainProcessor: IBlockchainProcessor
    {
        private readonly IBlockchainProcessingStrategy _strategy;
        private readonly BlockchainProcessorInstrumentation _log;

        public BlockchainProcessor(
            IBlockchainProcessingStrategy strategy,
            ILog log = null)
        {
            this._strategy = strategy;
            _log = new BlockchainProcessorInstrumentation(log);
        }

        /// <summary>
        /// Allow the processor to resume from where it left off
        /// </summary>
        private async Task<BigInteger> GetStartingBlockNumber()
        {
            _log.GettingLastBlockProcessed();
            var lastBlockProcessed = await _strategy.GetLastBlockProcessedAsync()
                .ConfigureAwait(false);

            _log.LastBlockProcessed(lastBlockProcessed);

            BigInteger startingBlock = lastBlockProcessed == null ? 0 : lastBlockProcessed.Value + 1;

            return _strategy.MinimumBlockNumber > startingBlock ? _strategy.MinimumBlockNumber : startingBlock;
        }

        /// <summary>
        /// Enumerates blocks on the chain.
        /// Invokes the ProcessBlockAsync method on the strategy for each block
        /// </summary>
        /// <param name="startBlock">Starting block - if null, will start from 0 or from where it left off
        /// (If the strategy provides for that)</param>
        /// <param name="endBlock">End block - if null, will run continuously and wait for new blocks</param>
        /// <returns>False if processing was cancelled else True</returns>
        public Task<bool> ExecuteAsync(
            BigInteger? startBlock, BigInteger? endBlock) => ExecuteAsync(startBlock, endBlock, new CancellationToken());

        public async Task<bool> ExecuteAsync(
            BigInteger? startBlock, BigInteger? endBlock, CancellationToken cancellationToken)
        {

            startBlock = startBlock ?? await GetStartingBlockNumber();

            if (endBlock.HasValue && startBlock.Value > endBlock.Value)
            {
                _log.StartBlockExceedsEndBlock(startBlock.Value, endBlock.Value);
                return false;
            }

            _log.BeginFillContractCacheAsync();
            await _strategy.FillContractCacheAsync().ConfigureAwait(false);


            _log.BeginningBlockEnumeration();
            return await new BlockEnumeration(
                    (blkNumber) => _strategy.ProcessBlockAsync(blkNumber), 
                    (retryNum) => _strategy.WaitForNextBlock(retryNum),
                    (retryNum) => _strategy.PauseFollowingAnError(retryNum), 
                    () => _strategy.GetMaxBlockNumberAsync(),
                    _strategy.MinimumBlockConfirmations,
                    _strategy.MaxRetries,
                    cancellationToken,
                    startBlock.Value, 
                    endBlock,
                    _log.Logger
                    )
                .ExecuteAsync().ConfigureAwait(false);
        }

        public Task ProcessAsync(BlockRange range) => ExecuteAsync(range.From, range.To, new CancellationToken());


        public Task ProcessAsync(BlockRange range, CancellationToken cancellationToken) => ExecuteAsync(range.From, range.To, cancellationToken);
       
    }
}