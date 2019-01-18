namespace Nethereum.BlockchainStore.Search
{
    public static class Extensions
    {
        public static SearchType InferResultType(this string query)
        {
            if (string.IsNullOrWhiteSpace(query)) return SearchType.Unknown;
            return RegularExpressions.InferSearchType(query.Trim());
        }
    }
}
