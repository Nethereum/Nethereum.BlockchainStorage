using System;

namespace Nethereum.BlockchainStore.Search
{
    public class SearchIndexDefinition
    {
        public SearchField[] Fields { get; set; } = Array.Empty<SearchField>();
        public string IndexName { get; protected set; }

        protected SearchIndexDefinition()
        {
        }

        public SearchIndexDefinition(string indexName)
        {
            IndexName = indexName;
        }
    }
}