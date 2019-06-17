using Common.Logging;
using System;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;

namespace Nethereum.BlockchainProcessing.Processing
{

    public class BlockEnumeration
    {
        private readonly Func<BigInteger, Task> _processBlock;
        private readonly Func<uint, Task> _waitForBlockAvailability;
        private readonly Func<uint, Task> _pauseFollowingAnError;
        private readonly Func<Task<BigInteger>> _getMaxBlockNumber;
        private readonly uint _minimumBlockConfirmations;
        private readonly BigInteger _endBlock;
        private readonly CancellationToken _cancellationToken;
        private readonly uint _maxRetries;
        private readonly bool _runContinuously;
        private BigInteger? _maxBlockNumber;

        private BigInteger _currentBlock;
        private uint _retryNumber;

        private readonly BlockEnumerationInstrumentation _log;

        public BlockEnumeration(
            Func<BigInteger, Task> processBlock,
            Func<uint, Task> waitForBlockAvailability,
            Func<uint, Task> pauseFollowingAnError,
            Func<Task<BigInteger>> getMaxBlockNumber,
            uint minimumBlockConfirmations,
            uint maxRetries,
            CancellationToken cancellationToken,
            BigInteger startBlock,
            BigInteger? endBlock = null,
            ILog log = null
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
            _endBlock = endBlock ?? ulong.MaxValue;
            _cancellationToken = cancellationToken;

            _log = new BlockEnumerationInstrumentation(log);
        }

        public async Task<bool> ExecuteAsync()
        {
            while (_currentBlock <= _endBlock)
            {
                try
                {
                    if (_cancellationToken.IsCancellationRequested) return false;

                    await WaitForBlockConfirmations().ConfigureAwait(false);

                    _log.LogProcessBlockAttempt(_currentBlock, _retryNumber);

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
                _log.WaitingForBlockAvailability(_currentBlock, _minimumBlockConfirmations, _maxBlockNumber, retryNumber);
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
            _log.LogBlockSkipped(_currentBlock);
        }

        private async Task<bool> WaitForNextBlockAndRetry()
        {
            _log.WaitingForBlock(_currentBlock);
            await _waitForBlockAvailability(_retryNumber).ConfigureAwait(false);
            _retryNumber++;
            return await ExecuteAsync().ConfigureAwait(false);
        }

        private void LogError(Exception exception)
        {
            _log.Error(_currentBlock, exception);
        }

        private async Task<bool> PauseAndRetry()
        {
            _log.PauseBeforeNextProcessingAttempt(_currentBlock, _retryNumber);
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
            _log.BlockIncremented(_currentBlock);
        }

    }

}