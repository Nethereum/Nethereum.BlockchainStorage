using Nethereum.BlockchainProcessing.Processor;
using Nethereum.RPC.Eth.DTOs;
using System;
using System.Threading.Tasks;

namespace Nethereum.BlockchainStore.Search
{
    public class SearchIndexProcessor : ProcessorHandler<FilterLog>
    {
        public SearchIndexProcessor(
            IIndexer<FilterLog> eventIndexer) :
        base((eventLog) => eventIndexer.IndexAsync(eventLog))
        { }

        public SearchIndexProcessor(
            IIndexer<FilterLog> eventIndexer,
            Func<FilterLog, Task<bool>> criteria) :
                base((eventLog) => eventIndexer.IndexAsync(eventLog), criteria)
        { }

        public SearchIndexProcessor(
            IIndexer<FilterLog> eventIndexer,
            Func<FilterLog, bool> criteria) :
                base((eventLog) => eventIndexer.IndexAsync(eventLog), criteria)
        { }
    }
}
