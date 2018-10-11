﻿using Common.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Nethereum.BlockchainStore.Processing
{
    public class BlockEnumeration
    {
        private readonly Func<long, Task> _processBlock;
        private readonly Func<int, Task> _waitForBlockAvailability;
        private readonly Func<int, Task> _pauseFollowingAnError;
        private readonly Func<Task<long>> getMaxBlockNumber;
        private readonly int minimumBlockConfirmations;
        private readonly long _endBlock;
        private readonly CancellationToken _cancellationToken;
        private readonly int _maxRetries;
        private readonly bool _runContinuously;
        private long? _maxBlockNumber;

        private long _currentBlock;
        private int _retryNumber;

        private readonly ILogger _log = ApplicationLogging.CreateLogger<BlockEnumeration>();

        public BlockEnumeration(
            Func<long, Task> processBlock,
            Func<int, Task> waitForBlockAvailability,
            Func<int, Task> pauseFollowingAnError,
            Func<Task<long>> getMaxBlockNumber,
            int minimumBlockConfirmations,
            int maxRetries,
            CancellationToken cancellationToken,
            long startBlock,
            long? endBlock = null
            )
        {
            _processBlock = processBlock;
            _waitForBlockAvailability = waitForBlockAvailability;
            _pauseFollowingAnError = pauseFollowingAnError;
            this.getMaxBlockNumber = getMaxBlockNumber;
            this.minimumBlockConfirmations = minimumBlockConfirmations;
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

                    await WaitForBlockConfirmations();

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

        private async Task WaitForBlockConfirmations()
        {
            if (minimumBlockConfirmations < 1) return;

            if (_maxBlockNumber == null)
                await RefreshMaxBlockNumber();

            int retryNumber = 0;
            while ((_maxBlockNumber - _currentBlock) < minimumBlockConfirmations)
            {
                _log.LogInformation($"Waiting for current block ({_currentBlock}) to be more than {minimumBlockConfirmations} confirmations behind the max block on the chain ({_maxBlockNumber})");
                await _waitForBlockAvailability(retryNumber);
                retryNumber++;
                await RefreshMaxBlockNumber();
            }
        }

        private async Task RefreshMaxBlockNumber()
        {
            _maxBlockNumber = await getMaxBlockNumber();
        }

        private void LogBlockSkipped()
        {
            _log?.LogWarning($"Skipping block {_currentBlock}");
        }

        private async Task<bool> WaitForNextBlockAndRetry()
        {
            _log?.LogInformation($"Waiting for block {_currentBlock}...");
            await _waitForBlockAvailability(_retryNumber);
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
            _log?.LogInformation($"Block Process Attempt.  Block: {_currentBlock}, Attempt: {_retryNumber}.");
        }
    }

}