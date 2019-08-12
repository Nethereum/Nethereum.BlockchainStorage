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
        where TSearchDocument : class
    {
        public ISearchIndexClient IndexClient {get;}

        public string Name => IndexClient.IndexName;

        protected AzureIndexerBase(ISearchIndexClient indexClient, Func<TSource, TSearchDocument> mapper, int documentsPerIndexBatch = 1)
            :base(mapper, documentsPerIndexBatch)
        {
            IndexClient = indexClient;
        }

        public override Task<long> DocumentCountAsync() => IndexClient.Documents.CountAsync();

        protected override Task SendBatchAsync(IEnumerable<TSearchDocument> docs) => ExecuteBatch(docs);

        protected virtual async Task ExecuteBatch<T>(IEnumerable<T> uploadOrMerge, IEnumerable<T> upload = null, IEnumerable<T> delete = null) 
            where T: class
        {
            var actions = new List<IndexAction<T>>();

            if (uploadOrMerge != null)
            {
                foreach (var item in uploadOrMerge)
                {
                    actions.Add(IndexAction.MergeOrUpload<T>(item));
                }
            }

            if (upload != null)
            {
                foreach (var item in upload)
                {
                    actions.Add(IndexAction.Upload<T>(item));
                }
            }

            if (delete != null)
            {
                foreach (var item in delete)
                {
                    actions.Add(IndexAction.Delete<T>(item));
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
            //IndexClient?.Dispose();
        }
    }
}