using Nest;
using Nethereum.Contracts;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nethereum.BlockchainStore.Search.ElasticSearch
{
    public class ElasticFunctionIndexer<TFunctionMessage, TSearchDocument>: 
        ElasticIndexerBase<TSearchDocument>,
        IFunctionIndexer<TFunctionMessage> where TFunctionMessage : FunctionMessage, new()
        where TSearchDocument : class, IHasId, new()
    {
        private readonly IFunctionMessageToSearchDocumentMapper<TFunctionMessage, TSearchDocument> _mapper;

        public ElasticFunctionIndexer(IElasticClient client, string indexName, IFunctionMessageToSearchDocumentMapper<TFunctionMessage, TSearchDocument> mapper) : base(client, indexName)
        {
            this._mapper = mapper;
        }

        public Task IndexAsync(TransactionForFunctionVO<TFunctionMessage> functionMessage) => IndexAsync(new[] {functionMessage});

        public async Task IndexAsync(IEnumerable<TransactionForFunctionVO<TFunctionMessage>> functionMessages)
        {
            var documents = functionMessages.Select(functionCall => _mapper.Map(functionCall)).ToArray();
            await BulkIndexAsync(documents);
        }
    }

    public class ElasticFunctionIndexer<TFunctionMessage>:
        ElasticIndexerBase<GenericElasticSearchDocument>,
        IFunctionIndexer<TFunctionMessage> where TFunctionMessage : FunctionMessage, new()
    {
        private readonly FunctionIndexDefinition<TFunctionMessage> _searchIndexDefinition;

        public ElasticFunctionIndexer(IElasticClient client, FunctionIndexDefinition<TFunctionMessage> searchIndexDefinition, string indexName = null) : base(client, indexName ?? searchIndexDefinition.IndexName)
        {
            _searchIndexDefinition = searchIndexDefinition;
        }

        public Task IndexAsync(TransactionForFunctionVO<TFunctionMessage> functionMessage) => IndexAsync(new[] {functionMessage});


        public async Task IndexAsync(IEnumerable<TransactionForFunctionVO<TFunctionMessage>> functionMessages)
        {
            var documents = functionMessages.Select(functionCall => functionCall.ToGenericElasticSearchDoc(_searchIndexDefinition)).ToArray();
            await BulkIndexAsync(documents);
        }
    }

}
