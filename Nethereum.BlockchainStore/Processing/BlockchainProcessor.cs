using Nethereum.BlockchainStore.Processors;
using NLog.Fluent;
using System;
using System.Threading.Tasks;

namespace Nethereum.BlockchainStore.Processing
{
    public class BlockchainProcessor
    {
        private const int MaxRetries = 3;
        private readonly IBlockchainProcessingStrategy _strategy;
        private readonly IBlockProcessor _blockProcessor;
        private readonly WaitForNextBlockStrategy _waitForNextBlockStrategy;
        private bool _contractCacheInitialised = false;

        public long? MinimumBlockNumber { get; set; }

        public BlockchainProcessor(
            IBlockchainProcessingStrategy strategy, 
            IBlockProcessor blockProcessor
            )
        {
            _waitForNextBlockStrategy = new WaitForNextBlockStrategy();
            this._strategy = strategy;
            this._blockProcessor = blockProcessor;
        }

        private async Task InitContractCache()
        {
            if (!_contractCacheInitialised)
            {
                await _strategy.FillContractCacheAsync().ConfigureAwait(false);
                _contractCacheInitialised = true;
            }
        }

        /// <summary>
        /// Allow the processor to resume from where it left off
        /// </summary>
        private async Task<long> GetStartingBlockNumber()
        {
            var blockNumber = await _strategy.GetMaxBlockNumberAsync();
            blockNumber = blockNumber <= 0 ? 0 : blockNumber - 1;

            if (MinimumBlockNumber.HasValue && MinimumBlockNumber > blockNumber)
                return MinimumBlockNumber.Value;

            return blockNumber;
        }

        public async Task<bool> ExecuteAsync(
            long? startBlock, long? endBlock)
        {
            startBlock = startBlock ?? await GetStartingBlockNumber();
            endBlock = endBlock ?? long.MaxValue;

            await InitContractCache();

            return await InternalExecuteAsync(startBlock.Value, endBlock.Value);
        }

        private async Task<bool> InternalExecuteAsync(
            long startBlock, long endBlock, int retryNumber = 0)
        {
            bool runContinuously = endBlock == long.MaxValue;
           
            while (startBlock <= endBlock)
                try
                {
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
                        await _waitForNextBlockStrategy.Apply(retryNumber);
                        await InternalExecuteAsync(startBlock, endBlock, retryNumber + 1);
                    }
                    else
                    {
                        if (retryNumber != MaxRetries)
                        {
                            await InternalExecuteAsync(startBlock, endBlock, retryNumber + 1)
                                .ConfigureAwait(false);
                        }
                        else
                        {
                            retryNumber = 0;
                            startBlock = startBlock + 1;
                            Log.Error().Exception(blockNotFoundException)
                                .Message("BlockNumber" + startBlock).Write();
                            System.Console.WriteLine($"Skipping block");
                        }
                    }
                }
                catch (Exception ex)
                {
                    System.Console.WriteLine(
                        ex.Message + ". " + ex.InnerException?.Message);

                    if (retryNumber != MaxRetries)
                    {
                        await InternalExecuteAsync(startBlock, endBlock, retryNumber + 1)
                            .ConfigureAwait(false);
                    }
                    else
                    {
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