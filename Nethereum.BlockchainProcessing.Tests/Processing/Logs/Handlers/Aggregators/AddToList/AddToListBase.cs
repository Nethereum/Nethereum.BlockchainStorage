using System.Threading.Tasks;

namespace Nethereum.BlockchainProcessing.Tests.Processing.Logs.Handlers.Aggregators.AddToList
{
    public abstract class AddToListBase: EventAggregatorTestsBase
    {
        public abstract Task CreatesAndAddsToList();
        public abstract Task AddsToExistingList();
    }
}

