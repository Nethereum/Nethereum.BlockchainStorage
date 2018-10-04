using NLog.Fluent;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Nethereum.BlockchainStore.Processing
{

    public class BlockEnumeration
    {
        private readonly Func<long, Task> _processBlock;
        private readonly Func<int, Task> _waitForBlockAvailability;
        private readonly Func<int, Task> _pauseFollowingAnError;
        private readonly long _endBlock;
        private readonly CancellationToken _cancellationToken;
        private readonly int _maxRetries;
        private readonly bool _runContinuously;

        private long _currentBlock;
        private int _retryNumber;

        public BlockEnumeration(
            Func<long, Task> processBlock,
            Func<int, Task> waitForBlockAvailability,
            Func<int, Task> pauseFollowingAnError,
            int maxRetries,
            CancellationToken cancellationToken,
            long startBlock,
            long? endBlock = null
            )
        {
            _processBlock = processBlock;
            _waitForBlockAvailability = waitForBlockAvailability;
            _pauseFollowingAnError = pauseFollowingAnError;
            _maxRetries = maxRetries;
            _currentBlock = startBlock;
            _runContinuously = endBlock == null;
            _endBlock = endBlock ?? long.MaxValue;
            _cancellationToken = cancellationToken;
        }

        public async Task<bool> ExecuteAsync()
        {
            while (_currentBlock <= _endBlock)
            {
                try
                {
                    if (_cancellationToken.IsCancellationRequested) return false;

                    LogProcessBlockAttempt();

                    await _processBlock(_currentBlock).ConfigureAwait(false);

                    IncrementBlockAndResetRetries();
                }
                catch (BlockNotFoundException blockNotFoundException)
                {
                    LogError(blockNotFoundException);

                    if (_runContinuously)
                        return await WaitForNextBlockAndRetry();

                    if (WithinRetryLimit())
                        return await PauseAndRetry();

                    LogBlockSkipped();
                    IncrementBlockAndResetRetries();
                }
                catch (Exception ex)
                {
                    LogError(ex);

                    if (WithinRetryLimit())
                        return await PauseAndRetry();

                    LogBlockSkipped();
                    IncrementBlockAndResetRetries();
                }
            }

            return true;
        }

        private static void LogBlockSkipped()
        {
            System.Console.WriteLine($"Skipping block");
        }

        private async Task<bool> WaitForNextBlockAndRetry()
        {
            System.Console.WriteLine("Waiting for block...");
            await _waitForBlockAvailability(_retryNumber);
            _retryNumber++;
            return await ExecuteAsync().ConfigureAwait(false);
        }

        private void LogError(Exception exception)
        {
            System.Console.WriteLine(exception.Message);

            Log.Error().Exception(exception)
                .Message("BlockNumber" + _currentBlock).Write();
        }

        private async Task<bool> PauseAndRetry()
        {
            await _pauseFollowingAnError(_retryNumber);
            _retryNumber++;
            return await ExecuteAsync().ConfigureAwait(false);
        }

        private bool WithinRetryLimit()
        {
            return _retryNumber != _maxRetries;
        }

        private void IncrementBlockAndResetRetries()
        {
            _retryNumber = 0;
            _currentBlock = _currentBlock + 1;
        }

        private void LogProcessBlockAttempt()
        {
            System.Console.WriteLine(
                $"{DateTime.Now.ToString("s")}. Block: {_currentBlock}. Attempt: {_retryNumber}");
        }
    }

}