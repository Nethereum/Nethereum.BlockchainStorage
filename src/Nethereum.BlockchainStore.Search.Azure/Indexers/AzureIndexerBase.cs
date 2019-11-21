using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;
using Microsoft.Rest.TransientFaultHandling;

namespace Nethereum.BlockchainStore.Search.Azure
{
    public abstract class AzureIndexerBase<TSource, TSearchDocument> : 
        IndexerBase<TSource, TSearchDocument> 
        where TSearchDocument : class, IHasId
    {
        public ISearchIndexClient IndexClient {get;}

        public string Name => IndexClient.IndexName;

        protected AzureIndexerBase(ISearchIndexClient indexClient, Func<TSource, TSearchDocument> mapper, int documentsPerIndexBatch = 1)
            :base(mapper, documentsPerIndexBatch)
        {
            IndexClient = indexClient;
        }

        public override Task<long> DocumentCountAsync() => IndexClient.Documents.CountAsync();

        protected override Task SendBatchAsync(IEnumerable<(DocumentIndexAction, TSearchDocument)> docs) => ExecuteBatch(docs);

        protected virtual async Task ExecuteBatch<T>(IEnumerable<(DocumentIndexAction action, T document)> documents) 
            where T: class
        {
            var actions = new List<IndexAction<T>>();

            foreach(var item in documents)
            {
                switch (item.action)
                {
                    case DocumentIndexAction.uploadOrMerge:
                        actions.Add(IndexAction.MergeOrUpload<T>(item.document));
                        break;
                    case DocumentIndexAction.upload:
                        actions.Add(IndexAction.Upload<T>(item.document));
                        break;
                    case DocumentIndexAction.delete:
                        actions.Add(IndexAction.Delete<T>(item.document));
                        break;
                }
            }

            var batch = IndexBatch.New(actions);

            var retryStrategy =
                new IncrementalRetryStrategy(3, TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(2));

            var retryPolicy =
                new RetryPolicy<SearchIndexErrorDetectionStrategy>(retryStrategy);
            //there is a retry policy for the client search now, we might be able to use that instead
            await retryPolicy.ExecuteAsync(async () => await IndexClient.Documents.IndexAsync(batch));
        }

        public override void Dispose()
        {
            base.Dispose();
        }
    }
}