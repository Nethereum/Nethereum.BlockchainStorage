using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;
using Nethereum.Contracts;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nethereum.BlockchainStore.Search.Azure
{

    public class AzureEventIndexer<TEvent, TSearchDocument>
    : AzureIndexBase, IAzureEventIndexer<TEvent>
        where TEvent : class where TSearchDocument : class, new()
    {
        private readonly IEventToSearchDocumentMapper<TEvent, TSearchDocument> _mapper;

        public AzureEventIndexer(Index index, ISearchIndexClient indexClient, IEventToSearchDocumentMapper<TEvent, TSearchDocument> mapper) : base(index, indexClient)
        {
            _mapper = mapper;
        }

        public Task IndexAsync(EventLog<TEvent> log) => IndexAsync(new[] {log});

        public async Task IndexAsync(IEnumerable<EventLog<TEvent>> logs)
        {
            var documents = logs.Select(l => _mapper.Map(l)).ToArray();
            await BatchUpdateAsync(documents);
            Indexed += documents.Length;
        }
    }

    public class AzureEventIndexer<TEvent> : 
        AzureIndexBase, IAzureEventIndexer<TEvent> 
        where TEvent : class
    {
        private readonly EventIndexDefinition<TEvent> _eventSearchDefinition;

        public AzureEventIndexer(
            EventIndexDefinition<TEvent> eventSearchDefinition, Index index, ISearchIndexClient indexClient)
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