using System;
using System.Threading.Tasks;

namespace Nethereum.BlockchainProcessing.Processing
{
    public static class ProcessingExtensions
    {
        public static async Task<(ulong from, ulong to)> GetBlockRangeToProcess(
            this IBlockchainProcessingProgressService _progressService, ulong _maxNumberOfBlocksPerBatch)
        {
            var from = await _progressService.GetBlockNumberToProcessFrom();
            var to = await _progressService.GetBlockNumberToProcessTo();

            if (to < from) 
                throw new InvalidOperationException($"Block number from ({from}) can not exceed block number to ({to}).");

            //process max 100 at a time?
            if ((to - from) > _maxNumberOfBlocksPerBatch)
            {
                to = from + _maxNumberOfBlocksPerBatch;
            }

            return (from, to);
        }
    }
}