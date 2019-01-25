using System.Linq;

namespace Nethereum.BlockchainStore.Search
{
    public static class SearchIndexExtensions
    {
        public static SearchField Field(this SearchIndexDefinition searchIndex, string name)
        {
            return searchIndex.Fields.FirstOrDefault(f => f.Name == name);
        }

        public static SearchField Field(this SearchIndexDefinition searchIndex, PresetSearchFieldName name)
        {
            return searchIndex.Field(name.ToString());
        }

        public static SearchField KeyField(this SearchIndexDefinition searchIndex)
        {
            return searchIndex.Fields.FirstOrDefault(f => f.IsKey);
        }

    }
}