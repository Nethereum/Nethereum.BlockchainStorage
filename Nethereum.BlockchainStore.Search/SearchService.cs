using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nethereum.BlockchainStore.Search
{

    public interface ISearchService
    {
        Task<SearchResult> FindFirstAsync(string query);
    }

    public enum SearchType
    {
        Unknown, Block, Transaction, Contract, Address
    }

    public class SearchService : ISearchService
    {
        private readonly ISearcherFactory _searcherFactory;

        public SearchService(ISearcherFactory searcherFactory)
        {
            _searcherFactory = searcherFactory;
        }

        public async Task<SearchResult> FindFirstAsync(string query)
        {
            foreach(var searcher in _searcherFactory.FindSearchers(query))
            {
                var result = await searcher.SearchAsync(query);
                if (result != null)
                {
                    return result;
                }
            }

            return SearchResult.Empty;
        }
    }

    public interface ISearcherFactory
    {
        IEnumerable<ISearcher> FindSearchers(string query);
    }

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
                {SearchType.Unknown, new ISearcher[] {new UnknownSearchTypeSearcher() }},
                {SearchType.Block, new ISearcher[] {blockSearcher}},
                {SearchType.Transaction, new ISearcher[] {transactionSearcher}},
                {SearchType.Address, new ISearcher[] {contractSearcher, addressSearcher}}
            };
        }

        public IEnumerable<ISearcher> FindSearchers(string query)
        {
            var searchType = query.InferResultType();
            return Searchers[searchType];
        }
    }

    public interface ISearcher
    {
        Task<SearchResult> SearchAsync(string query);
    }

    public class UnknownSearchTypeSearcher : ISearcher
    {
        public Task<SearchResult> SearchAsync(string query)
        {
            return Task.FromResult(new SearchResult() {Title = $"Invalid or unsupported query. ({query})"});
        }
    }

    public class BlockSearcher : ISearcher
    {
        public Task<SearchResult> SearchAsync(string query)
        {
            throw new NotImplementedException();
        }
    }

    public class TransactionSearcher : ISearcher
    {
        public Task<SearchResult> SearchAsync(string query)
        {
            throw new NotImplementedException();
        }
    }

    public class ContractSearcher : ISearcher
    {
        public Task<SearchResult> SearchAsync(string query)
        {
            throw new NotImplementedException();
        }
    }

    public class AddressSearcher : ISearcher
    {
        public Task<SearchResult> SearchAsync(string query)
        {
            throw new NotImplementedException();
        }
    }

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

    public class Block
    {
        public string Number { get; set; }
    }

    public class Transaction
    {
        public string Hash { get; set; }
    }

    public class Address: AddressBase
    {
        
    }

    public class Contract: AddressBase
    {
        
    }

    public abstract class AddressBase
    {
        public string Address { get; set; }
    }
}
