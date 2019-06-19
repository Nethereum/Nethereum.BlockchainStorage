using System.Threading.Tasks;

namespace Nethereum.BlockchainProcessing.Tests.Processing.Logs.Handlers.Aggregators.Sum
{
    public abstract class SumTestsBase: EventAggregatorTestsBase
    {
        public abstract Task CreatesAndIncrementsSum();
        public abstract Task IncrementsExistingSum();
    }
}

