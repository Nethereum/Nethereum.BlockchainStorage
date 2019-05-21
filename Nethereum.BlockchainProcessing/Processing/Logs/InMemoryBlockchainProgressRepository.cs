using System.Threading.Tasks;

namespace Nethereum.BlockchainProcessing.Processing.Logs
{
    public class InMemoryBlockchainProgressRepository : IBlockProgressRepository
    {
        public InMemoryBlockchainProgressRepository(ulong startingBlockNumber)
        {
            StartingBlockNumber = startingBlockNumber;
            CurrentBlockNumber = StartingBlockNumber;
        }

        public ulong StartingBlockNumber { get; }

        public ulong CurrentBlockNumber { get; private set;}

        public Task<ulong?> GetLastBlockNumberProcessedAsync() => Task.FromResult((ulong?)CurrentBlockNumber);

        public Task UpsertProgressAsync(ulong blockNumber)
        {
            CurrentBlockNumber = CurrentBlockNumber + 1;
            return Task.CompletedTask;
        }
    }
}
