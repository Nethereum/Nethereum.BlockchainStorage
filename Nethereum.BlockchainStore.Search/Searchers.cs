using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Nethereum.BlockchainStore.Search
{
    public class UnknownSearchTypeSearcher : ISearcher
    {
        public static readonly UnknownSearchTypeSearcher Instance = new UnknownSearchTypeSearcher();

        public Task<SearchResult> FindFirstAsync(string query)
        {
            return Task.FromResult(new SearchResult() {Title = $"Invalid or unsupported query. ({query})"});
        }
    }

    public class BlockSearcher : ISearcher
    {
        public Task<SearchResult> FindFirstAsync(string query)
        {
            throw new NotImplementedException();
        }
    }

    public class TransactionSearcher : ISearcher
    {
        public Task<SearchResult> FindFirstAsync(string query)
        {
            throw new NotImplementedException();
        }
    }

    public class ContractSearcher : ISearcher
    {
        public Task<SearchResult> FindFirstAsync(string query)
        {
            throw new NotImplementedException();
        }
    }

    public class AddressSearcher : ISearcher
    {
        public Task<SearchResult> FindFirstAsync(string query)
        {
            throw new NotImplementedException();
        }
    }
}
