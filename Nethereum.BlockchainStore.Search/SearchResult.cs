namespace Nethereum.BlockchainStore.Search
{
    public class SearchResult
    {
        public static readonly SearchResult Empty = new SearchResult{Title = "No results were found"};

        public string Title { get; set; }
        public SearchType Type { get; set; }
        public Block Block { get; set; }
        public Transaction Transaction { get; set; }
        public Contract Contract { get; set; }
        public Address Address { get; set; }
    }
}
