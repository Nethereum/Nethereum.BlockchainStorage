using Nethereum.BlockchainProcessing.Processing;
using Nethereum.BlockchainProcessing.Processors;
using System.Numerics;
using System.Threading.Tasks;

namespace Nethereum.BlockchainStore.Repositories
{

    public class StorageProcessingStrategy: ProcessingStrategy, IBlockchainProcessingStrategy
    {
        private readonly RepositoryHandlerContext _repositoryHandlerContext;

        public StorageProcessingStrategy(
            RepositoryHandlerContext repositoryHandlerContext, 
            IBlockProcessor blockProcessor):base(blockProcessor)
        {
            _repositoryHandlerContext = repositoryHandlerContext;
        }

        public override async Task<BigInteger?> GetLastBlockProcessedAsync()
        {
            return await _repositoryHandlerContext.BlockRepository.GetMaxBlockNumberAsync().ConfigureAwait(false);
        }

        public override async Task FillContractCacheAsync()
        {
            await _repositoryHandlerContext.ContractRepository.FillCache().ConfigureAwait(false);
        }
    }
}
