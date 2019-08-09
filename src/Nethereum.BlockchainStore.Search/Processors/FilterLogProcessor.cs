using Nethereum.BlockchainProcessing.Processor;
using Nethereum.RPC.Eth.DTOs;
using System;
using System.Threading.Tasks;

namespace Nethereum.BlockchainStore.Search
{
    public class FilterLogProcessor : ProcessorHandler<FilterLog>
    {
        public FilterLogProcessor(
            IIndexer<FilterLog> eventIndexer) :
        base((eventLog) => eventIndexer.IndexAsync(eventLog))
        {
            EventIndexer = eventIndexer;
        }

        public FilterLogProcessor(
            IIndexer<FilterLog> eventIndexer,
            Func<FilterLog, Task<bool>> asyncCriteria) :
                base((eventLog) => eventIndexer.IndexAsync(eventLog), asyncCriteria)
        {
            EventIndexer = eventIndexer;
        }

        public FilterLogProcessor(
            IIndexer<FilterLog> eventIndexer,
            Func<FilterLog, bool> criteria) :
                base((eventLog) => eventIndexer.IndexAsync(eventLog), criteria)
        {
            EventIndexer = eventIndexer;
        }

        public IIndexer<FilterLog> EventIndexer { get; }
    }
}
