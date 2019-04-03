using System.Threading.Tasks;

namespace Nethereum.BlockchainProcessing.Tests.Processing.Logs.Handlers.Aggregators.Count
{
    public abstract class CountTestsBase: EventAggregatorTestsBase
    {
        public abstract Task CreatesAndIncrementsCounter();
        public abstract Task IncrementsExistingCounter();
    }
}

