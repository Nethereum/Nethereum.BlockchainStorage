using System.Threading.Tasks;

namespace Nethereum.BlockchainProcessing.Processing
{
    public abstract class BlockchainProcessingProgressService: IBlockchainProcessingProgressService
    {
        protected ulong DefaultStartingBlockNumber;
        private readonly IBlockProcessProgressRepository _blockProcessProgressRepository;

        public BlockchainProcessingProgressService(
            ulong defaultStartingBlockNumber, 
            IBlockProcessProgressRepository blockProcessProgressRepository)
        {
            DefaultStartingBlockNumber = defaultStartingBlockNumber;
            _blockProcessProgressRepository = blockProcessProgressRepository;
        }

        public async Task UpsertBlockNumberProcessedTo(ulong blockNumber)
        {
            await _blockProcessProgressRepository.UpsertProgressAsync(blockNumber).ConfigureAwait(false);
        }

        public async Task<ulong> GetBlockNumberToProcessFrom()
        {
            var lastBlockProcessed = await _blockProcessProgressRepository.GetLatestAsync().ConfigureAwait(false);

            var blockNumber = DefaultStartingBlockNumber;

            if (lastBlockProcessed != null)
            {
                //last block plus one
                blockNumber = lastBlockProcessed.Value + 1;
            }
            return blockNumber;
        }

        public abstract Task<ulong> GetBlockNumberToProcessTo();

        public virtual async Task<BlockRange?> GetNextBlockRangeToProcess(uint maxNumberOfBlocksPerBatch)
        {
            var from = await GetBlockNumberToProcessFrom();
            var to = await GetBlockNumberToProcessTo();

            if (to < from) 
                return null;

            //process max 100 at a time?
            if ((to - from) > maxNumberOfBlocksPerBatch)
            {
                to = from + maxNumberOfBlocksPerBatch;
            }

            return new BlockRange(from, to);
        }
    }
}
