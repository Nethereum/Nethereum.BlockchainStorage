using System.Threading.Tasks;

namespace Nethereum.BlockchainProcessing.Processing
{
    public abstract class BlockProgressServiceBase: IBlockProgressService
    {
        protected ulong DefaultStartingBlockNumber;
        private readonly IBlockProgressRepository _blockProgressRepository;

        protected BlockProgressServiceBase(
            ulong defaultStartingBlockNumber, 
            IBlockProgressRepository blockProgressRepository)
        {
            DefaultStartingBlockNumber = defaultStartingBlockNumber;
            _blockProgressRepository = blockProgressRepository;
        }

        public virtual async Task SaveLastBlockProcessedAsync(ulong blockNumber)
        {
            await _blockProgressRepository
                .UpsertProgressAsync(blockNumber)
                .ConfigureAwait(false);
        }

        public virtual async Task<ulong> GetBlockNumberToProcessFrom()
        {
            var lastBlockProcessed = await _blockProgressRepository
                .GetLastBlockNumberProcessedAsync()
                .ConfigureAwait(false);

            var blockNumber = DefaultStartingBlockNumber;

            if (lastBlockProcessed != null)
            {
                //last block plus one
                blockNumber = lastBlockProcessed.Value + 1;
            }
            return blockNumber;
        }

        public abstract Task<ulong> GetBlockNumberToProcessTo();

        public virtual async Task<BlockRange?> GetNextBlockRangeToProcessAsync(
            uint maxNumberOfBlocksPerBatch)
        {
            var from = await GetBlockNumberToProcessFrom().ConfigureAwait(false);
            var to = await GetBlockNumberToProcessTo().ConfigureAwait(false);

            if (to < from) 
                return null;

            if ((to - from) > maxNumberOfBlocksPerBatch)
            {
                to = from + maxNumberOfBlocksPerBatch;
            }

            return new BlockRange(from, to);
        }
    }
}
