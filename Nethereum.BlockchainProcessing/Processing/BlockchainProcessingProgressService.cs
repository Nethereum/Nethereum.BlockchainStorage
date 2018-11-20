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
            var processInfo = await _blockProcessProgressRepository.GetLatestAsync().ConfigureAwait(false);

            var blockNumber = DefaultStartingBlockNumber;

            if (processInfo != null)
            {
                blockNumber = processInfo.Value;
            }
            return blockNumber;
        }

        public abstract Task<ulong> GetBlockNumberToProcessTo();

    }
}
