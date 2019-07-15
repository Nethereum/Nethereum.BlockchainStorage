using System.Collections.Generic;

namespace Nethereum.BlockchainStore.Search.RepositorySearching
{
    public class SearcherFactory: ISearcherFactory
    {
        public Dictionary<SearchType, ISearcher[]> Searchers;

        public SearcherFactory(
            ISearcher blockSearcher, 
            ISearcher transactionSearcher, 
            ISearcher contractSearcher, 
            ISearcher addressSearcher)
        {
            Searchers = new Dictionary<SearchType, ISearcher[]>
            {
                {SearchType.Unknown, new ISearcher[] { UnknownSearchTypeSearcher.Instance }},
                {SearchType.Block, new ISearcher[] {blockSearcher}},
                {SearchType.Transaction, new ISearcher[] {transactionSearcher}},
                {SearchType.Address, new ISearcher[] {contractSearcher, addressSearcher}}
            };
        }

        public ISearcher[] FindSearchers(string query)
        {
            var searchType = query.InferResultType();
            return Searchers[searchType];
        }
    }
}
