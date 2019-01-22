using Nethereum.BlockchainStore.Entities;

namespace Nethereum.BlockchainStore.Search
{
    public class SearchResult
    {
        public static readonly SearchResult Empty = new SearchResult{Title = "No results were found"};

        public string Title { get; set; }
        public SearchType Type { get; set; }
        public IBlockView Block { get; set; }
        public ITransactionView Transaction { get; set; }
        public IContractView Contract { get; set; }
    }
}
