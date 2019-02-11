using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;
using Nethereum.Contracts;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Nethereum.Contracts.Extensions;

namespace Nethereum.BlockchainStore.Search.Azure
{
    public class AzureEventSearchSearchIndex<TEvent> : AzureSearchIndexBase, IAzureEventSearchIndex<TEvent> where TEvent : class
    {
        private readonly EventIndexDefinition<TEvent> _eventSearchDefinition;

        public AzureEventSearchSearchIndex(EventIndexDefinition<TEvent> eventSearchDefinition, Index index, ISearchIndexClient indexClient)
        :base(index, indexClient)
        {
            _eventSearchDefinition = eventSearchDefinition;
        }

        public Task IndexAsync(EventLog<TEvent> log) => IndexAsync(new[] {log});

        public async Task IndexAsync(IEnumerable<EventLog<TEvent>> logs)
        {
            var documents = logs.Select(l => l.ToAzureDocument(_eventSearchDefinition)).ToArray();
            await BatchUpdateAsync(documents);
            Indexed += documents.Length;
        }
    }
}