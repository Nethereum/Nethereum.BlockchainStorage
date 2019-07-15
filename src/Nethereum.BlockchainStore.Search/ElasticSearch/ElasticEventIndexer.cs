using Nest;
using Nethereum.Contracts;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nethereum.BlockchainStore.Search.ElasticSearch
{
    public class ElasticEventIndexer<TEvent, TSearchDocument>
        : ElasticIndexerBase<TSearchDocument>, IEventIndexer<TEvent>
        where TEvent : class where TSearchDocument : class, IHasId, new()
    {
        private readonly IEventToSearchDocumentMapper<TEvent, TSearchDocument> _mapper;

        public ElasticEventIndexer(IElasticClient client, 
            string indexName,
            IEventToSearchDocumentMapper<TEvent, TSearchDocument> mapper):base(client, indexName)
        {
            _mapper = mapper;
        }

        public Task IndexAsync(EventLog<TEvent> log) => IndexAsync(new[] {log});

        public async Task IndexAsync(IEnumerable<EventLog<TEvent>> logs)
        {
            var documents = logs.Select(log => _mapper.Map(log)).ToArray();
            await BulkIndexAsync(documents);
        }

    }

    public class ElasticEventIndexer<TEvent> : 
        ElasticIndexerBase<GenericElasticSearchDocument>, IEventIndexer<TEvent> 
        where TEvent : class
    {
        private readonly EventIndexDefinition<TEvent> _eventIndexDefinition;

        public ElasticEventIndexer(
            IElasticClient client, 
            EventIndexDefinition<TEvent> eventIndexDefinition, 
            string indexName = null)
            :base(client, indexName ?? eventIndexDefinition.IndexName)
        {
            _eventIndexDefinition = eventIndexDefinition;
        }

        public Task IndexAsync(EventLog<TEvent> log) => IndexAsync(new[] {log});

        public async Task IndexAsync(IEnumerable<EventLog<TEvent>> logs)
        {
            var documents = logs.Select(log => log.ToGenericElasticSearchDoc(_eventIndexDefinition)).ToArray();
            await BulkIndexAsync(documents);
        }
    }
}
