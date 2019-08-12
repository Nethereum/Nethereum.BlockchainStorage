using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Nest;

namespace Nethereum.BlockchainStore.Search.ElasticSearch
{
    public class ElasticIndexerBase<TSource, TSearchDocument> :
        IndexerBase<TSource, TSearchDocument>
        where TSearchDocument : class, IHasId
    { 
        protected readonly IElasticClient Client;
        protected readonly string IndexName;

        protected ElasticIndexerBase(string indexName, IElasticClient client, Func<TSource, TSearchDocument> mapper, int documentsPerIndexBatch = 1)
            :base(mapper, documentsPerIndexBatch)
        {
            Client = client;
            IndexName = indexName.ToElasticName();
        }

        public override async Task<long> DocumentCountAsync()
        {
            var countResponse = await Client.CountAsync<TSearchDocument>(new CountRequest(IndexName));
            return countResponse.Count;
        }


        protected override async Task SendBatchAsync(IEnumerable<TSearchDocument> documents)
        {
            var request = new BulkRequest(IndexName, "_doc")
            {
                Operations = new List<IBulkOperation>(documents.Select(doc =>
                    new BulkIndexOperation<TSearchDocument>(doc) { Id = doc.GetId() }))
            };

            var bulkResponse = await Client.BulkAsync(request);
            if (bulkResponse.Errors)
            {
                throw new Exception(string.Join(",", bulkResponse.ItemsWithErrors.Select(i => i.Error.Type)));
            }

            if (!bulkResponse.IsValid)
            {
                throw new Exception(bulkResponse.OriginalException.Message, bulkResponse.OriginalException);
            }
        }
    }
}