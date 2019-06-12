using Nethereum.BlockchainProcessing.Processing;

namespace Nethereum.LogProcessing
{
    public class LogBatchProcessedArgs
    {
        public LogBatchProcessedArgs()
        {

        }

        public LogBatchProcessedArgs(uint batchesProcessedSoFar, BlockRange lastRangeProcessed)
        {
            BatchesProcessedSoFar = batchesProcessedSoFar;
            LastRangeProcessed = lastRangeProcessed;
        }

        public uint BatchesProcessedSoFar {get; }
        public BlockRange LastRangeProcessed { get;}
    }
}