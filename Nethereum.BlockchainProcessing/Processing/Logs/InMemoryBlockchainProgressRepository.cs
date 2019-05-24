using System.Threading.Tasks;

namespace Nethereum.BlockchainProcessing.Processing.Logs
{
    public class InMemoryBlockchainProgressRepository : IBlockProgressRepository
    {
        public InMemoryBlockchainProgressRepository(ulong? lastBlockProcessed)
        {
            LastBlockProcessed = lastBlockProcessed;
        }

        public ulong? LastBlockProcessed { get; private set;}

        public Task<ulong?> GetLastBlockNumberProcessedAsync() => Task.FromResult((ulong?)LastBlockProcessed);

        public Task UpsertProgressAsync(ulong blockNumber)
        {
            LastBlockProcessed = blockNumber;
            return Task.CompletedTask;
        }
    }
}
