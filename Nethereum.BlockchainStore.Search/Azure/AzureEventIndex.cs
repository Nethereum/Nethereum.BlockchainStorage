using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;
using Microsoft.Rest.TransientFaultHandling;
using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Contracts;

namespace Nethereum.BlockchainStore.Search.Azure
{
    public class AzureEventIndex<TEvent> : IDisposable, IAzureEventIndex<TEvent> where TEvent : class, IEventDTO, new()
    {
        private readonly EventSearchIndexDefinition<TEvent> eventSearchDefinition;
        private readonly Index index;
        private readonly ISearchIndexClient indexClient;

        public AzureEventIndex(EventSearchIndexDefinition<TEvent> eventSearchDefinition, Index index, ISearchIndexClient indexClient)
        {
            this.eventSearchDefinition = eventSearchDefinition;
            this.index = index;
            this.indexClient = indexClient;
        }

        public async Task UpsertAsync(EventLog<TEvent> log)
        {
            var document = log.ToAzureDocument(eventSearchDefinition);
            await BatchUpdateAsync(new[] {document});
        }

        public async Task<DocumentSuggestResult<Dictionary<string, object>>> SuggestAsync(string searchText, bool fuzzy = true)
        {
            var sp = new SuggestParameters
            {
                UseFuzzyMatching = fuzzy,
                Top = 8
            };

            return await indexClient
                .Documents
                .SuggestAsync<Dictionary<string, object>>(searchText, AzureSearchExtensions.SuggesterName, sp);
        }

        public async Task<DocumentSearchResult<Dictionary<string, object>>> SearchAsync(string text, IList<string> facets = null)
        {
            var sp = new SearchParameters
            {
                SearchMode = SearchMode.All,
                Top = 20,
                Facets = facets ?? index.FacetableFieldNames(),
                IncludeTotalResultCount = true
            };

            return await indexClient
                .Documents
                .SearchAsync<Dictionary<string, object>>(text, sp);
        }

        private async Task BatchUpdateAsync<T>(IEnumerable<T> uploadOrMerge, IEnumerable<T> upload = null, IEnumerable<T> delete = null) where T : class
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
            await retryPolicy.ExecuteAsync(async () => await indexClient.Documents.IndexAsync(batch));
        }

        public void Dispose()
        {
            indexClient?.Dispose();
        }

    }
}