using Nethereum.BlockchainStore.Processors;
using NLog.Fluent;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Nethereum.BlockchainStore.Processing
{
    public class BlockchainProcessor
    {
        private readonly IBlockchainProcessingStrategy _strategy;
        private readonly IBlockProcessor _blockProcessor;

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
            endBlock = endBlock ?? long.MaxValue;

            if (startBlock.Value > endBlock.Value)
                return false;

            await _strategy.FillContractCacheAsync().ConfigureAwait(false);

            return await InternalExecuteAsync(startBlock.Value, endBlock.Value, cancellationToken);
        }

        private async Task<bool> InternalExecuteAsync(
            long startBlock, long endBlock, CancellationToken cancellationToken, int retryNumber = 0)
        {
            bool runContinuously = endBlock == long.MaxValue;

            while (startBlock <= endBlock)
            {
                try
                {
                    if (cancellationToken.IsCancellationRequested) return false;

                    System.Console.WriteLine(
                        $"{DateTime.Now.ToString("s")}. Block: {startBlock}. Attempt: {retryNumber}");

                    await _blockProcessor.ProcessBlockAsync(startBlock).ConfigureAwait(false);
                    retryNumber = 0;
                    startBlock = startBlock + 1;
                }
                catch (BlockNotFoundException blockNotFoundException)
                {
                    System.Console.WriteLine(blockNotFoundException.Message);

                    if (runContinuously)
                    {
                        System.Console.WriteLine("Waiting for block...");
                        await _strategy.WaitForNextBlock(retryNumber);
                        return await InternalExecuteAsync(startBlock, endBlock, cancellationToken, retryNumber + 1);
                    }

                    if (retryNumber != _strategy.MaxRetries)
                    {
                        await _strategy.PauseFollowingAnError(retryNumber);
                        return await InternalExecuteAsync(startBlock, endBlock, cancellationToken, retryNumber + 1)
                            .ConfigureAwait(false);
                    }

                    retryNumber = 0;
                    startBlock = startBlock + 1;
                    Log.Error().Exception(blockNotFoundException)
                        .Message("BlockNumber" + startBlock).Write();
                    System.Console.WriteLine($"Skipping block");
                    
                }
                catch (Exception ex)
                {
                    System.Console.WriteLine(
                        ex.Message + ". " + ex.InnerException?.Message);

                    if (retryNumber != _strategy.MaxRetries)
                    {
                        await _strategy.PauseFollowingAnError(retryNumber);
                        return await InternalExecuteAsync(startBlock, endBlock, cancellationToken, retryNumber + 1)
                            .ConfigureAwait(false);
                    }
 
                    retryNumber = 0;
                    startBlock = startBlock + 1;
                    Log.Error().Exception(ex).Message(
                        "BlockNumber" + startBlock).Write();
                    System.Console.WriteLine(
                        "ERROR:" + startBlock + " " + DateTime.Now.ToString("s"));
                }
            }

            return true;
        }
    }
}