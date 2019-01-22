namespace Nethereum.BlockchainStore.Search
{
    public static class SearchExtensions
    {
        public static SearchType InferResultType(this string query)
        {
            if (string.IsNullOrWhiteSpace(query)) return SearchType.Unknown;
            return SearchQueryParser.InferSearchType(query.Trim());
        }
    }
}
