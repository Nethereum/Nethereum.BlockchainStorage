using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Nethereum.Configuration;

namespace Nethereum.BlockchainProcessing.Processing
{
    public class BlockEnumeration
    {
        private readonly Func<ulong, Task> _processBlock;
        private readonly Func<uint, Task> _waitForBlockAvailability;
        private readonly Func<uint, Task> _pauseFollowingAnError;
        private readonly Func<Task<ulong>> _getMaxBlockNumber;
        private readonly uint _minimumBlockConfirmations;
        private readonly ulong _endBlock;
        private readonly CancellationToken _cancellationToken;
        private readonly uint _maxRetries;
        private readonly bool _runContinuously;
        private ulong? _maxBlockNumber;

        private ulong _currentBlock;
        private uint _retryNumber;

        private readonly ILogger _log = ApplicationLogging.CreateLogger<BlockEnumeration>();

        public BlockEnumeration(
            Func<ulong, Task> processBlock,
            Func<uint, Task> waitForBlockAvailability,
            Func<uint, Task> pauseFollowingAnError,
            Func<Task<ulong>> getMaxBlockNumber,
            uint minimumBlockConfirmations,
            uint maxRetries,
            CancellationToken cancellationToken,
            ulong startBlock,
            ulong? endBlock = null
            )
        {
            _processBlock = processBlock;
            _waitForBlockAvailability = waitForBlockAvailability;
            _pauseFollowingAnError = pauseFollowingAnError;
            _getMaxBlockNumber = getMaxBlockNumber;
            _minimumBlockConfirmations = minimumBlockConfirmations;
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

                    await WaitForBlockConfirmations().ConfigureAwait(false);

                    LogProcessBlockAttempt();

                    await _processBlock(_currentBlock).ConfigureAwait(false);

                    IncrementBlockAndResetRetries();
                }
                catch (BlockNotFoundException blockNotFoundException)
                {
                    LogError(blockNotFoundException);

                    if (_runContinuously)
                        return await WaitForNextBlockAndRetry().ConfigureAwait(false);

                    if (WithinRetryLimit())
                        return await PauseAndRetry().ConfigureAwait(false);

                    LogBlockSkipped();
                    IncrementBlockAndResetRetries();
                }
                catch (Exception ex)
                {
                    LogError(ex);

                    if (WithinRetryLimit())
                        return await PauseAndRetry().ConfigureAwait(false);

                    LogBlockSkipped();
                    IncrementBlockAndResetRetries();
                }
            }

            return true;
        }

        private async Task WaitForBlockConfirmations()
        {
            if (_minimumBlockConfirmations < 1) return;

            if (_maxBlockNumber == null)
                await RefreshMaxBlockNumber().ConfigureAwait(false);

            uint retryNumber = 0;
            while ((_maxBlockNumber - _currentBlock) < _minimumBlockConfirmations)
            {
                _log.LogInformation($"Waiting for current block ({_currentBlock}) to be more than {_minimumBlockConfirmations} confirmations behind the max block on the chain ({_maxBlockNumber})");
                await _waitForBlockAvailability(retryNumber).ConfigureAwait(false);
                retryNumber++;
                await RefreshMaxBlockNumber().ConfigureAwait(false);
            }
        }

        private async Task RefreshMaxBlockNumber()
        {
            _maxBlockNumber = await _getMaxBlockNumber().ConfigureAwait(false);
        }

        private void LogBlockSkipped()
        {
            _log?.LogWarning($"Skipping block {_currentBlock}");
        }

        private async Task<bool> WaitForNextBlockAndRetry()
        {
            _log?.LogInformation($"Waiting for block {_currentBlock}...");
            await _waitForBlockAvailability(_retryNumber).ConfigureAwait(false);
            _retryNumber++;
            return await ExecuteAsync().ConfigureAwait(false);
        }

        private void LogError(Exception exception)
        {
            _log?.LogError($"Block: {_currentBlock}.  {exception.Message}", exception);
        }

        private async Task<bool> PauseAndRetry()
        {
            _log?.LogInformation($"Pausing before next process Attempt.  Block: {_currentBlock}, Attempt: {_retryNumber}.");
            await _pauseFollowingAnError(_retryNumber).ConfigureAwait(false);
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
            _log?.LogInformation($"Block Process Attempt.  Block: {_currentBlock}, Attempt: {_retryNumber}.");
        }
    }

}