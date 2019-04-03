using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;
using Microsoft.Rest.TransientFaultHandling;

namespace Nethereum.BlockchainStore.Search.Azure
{
    public abstract class AzureIndexBase: IAzureIndex
    {
        protected readonly Index Index;
        protected readonly ISearchIndexClient IndexClient;

        public int Indexed { get; protected set; }

        public string Name => Index.Name;

        protected AzureIndexBase(Index index, ISearchIndexClient indexClient)
        {
            Index = index;
            IndexClient = indexClient;
        }

        public virtual async Task<DocumentSuggestResult<Dictionary<string, object>>> SuggestAsync(string searchText, bool fuzzy = true)
        {
            var sp = new SuggestParameters
            {
                UseFuzzyMatching = fuzzy,
                Top = 8
            };

            return await IndexClient
                .Documents
                .SuggestAsync<Dictionary<string, object>>(searchText, AzureEventSearchExtensions.SuggesterName, sp);
        }

        public virtual async Task<DocumentSearchResult<Dictionary<string, object>>> SearchAsync(string text, IList<string> facets = null)
        {
            var sp = new SearchParameters
            {
                SearchMode = SearchMode.All,
                Top = 20,
                Facets = facets ?? Index.FacetableFieldNames(),
                IncludeTotalResultCount = true
            };

            return await IndexClient
                .Documents
                .SearchAsync<Dictionary<string, object>>(text, sp);
        }

        public virtual Task<long> DocumentCountAsync() => IndexClient.Documents.CountAsync();

        protected virtual async Task BatchUpdateAsync<T>(IEnumerable<T> uploadOrMerge, IEnumerable<T> upload = null, IEnumerable<T> delete = null) where T : class
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

        public virtual void Dispose()
        {
            IndexClient?.Dispose();
        }
    }
}