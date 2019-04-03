using System;
using System.Threading.Tasks;
using Moq;
using Nethereum.BlockchainStore.Search.RepositorySearching;
using Xunit;

namespace Nethereum.BlockchainStore.Search.Tests.RepositorySearching
{
    public class SearchServiceTests
    {
        [Fact]
        public async Task FindFirst_InvokesSearchersUntilANonNullResultIsReturned()
        {
            var searchFactoryMock = new Mock<ISearcherFactory>();
            var service = new SearchService(searchFactoryMock.Object);

            const string query = "76456";

            var firstSearcher = new Mock<ISearcher>();
            var secondSearcher = new Mock<ISearcher>();
            var searchers = new []{firstSearcher.Object, secondSearcher.Object};

            SearchResult nonNullSearchResult = new SearchResult();

            searchFactoryMock
                .Setup(f => f.FindSearchers(query))
                .Returns(searchers);

            firstSearcher
                .Setup(s => s.FindFirstAsync(query))
                .ReturnsAsync(null as SearchResult);

            secondSearcher
                .Setup(s => s.FindFirstAsync(query))
                .ReturnsAsync(nonNullSearchResult);

            var actualResult = await service.FindFirstAsync(query);

            Assert.Equal(nonNullSearchResult, actualResult);
        }

        [Fact]
        public async Task FindFirst_WhenThereAreNoMatchingSearchersReturnsAnEmptyResult()
        {
            var searchFactoryMock = new Mock<ISearcherFactory>();
            var service = new SearchService(searchFactoryMock.Object);

            const string query = "76456";

            var emptySearcherArray = Array.Empty<ISearcher>();

            searchFactoryMock
                .Setup(f => f.FindSearchers(query))
                .Returns(emptySearcherArray);

            Assert.Equal(SearchResult.Empty, await service.FindFirstAsync(query));
        }


        [Fact]
        public async Task FindFirst_WhenAllSearchersReturnNullReturnsAnEmptyResult()
        {
            var searchFactoryMock = new Mock<ISearcherFactory>();
            var service = new SearchService(searchFactoryMock.Object);

            const string query = "76456";

            var firstSearcher = new Mock<ISearcher>();
            var secondSearcher = new Mock<ISearcher>();
            var emptySearcherArray = new [] {firstSearcher.Object, secondSearcher.Object};

            searchFactoryMock
                .Setup(f => f.FindSearchers(query))
                .Returns(emptySearcherArray);

            firstSearcher.Setup(s => s.FindFirstAsync(query)).ReturnsAsync(null as SearchResult);
            secondSearcher.Setup(s => s.FindFirstAsync(query)).ReturnsAsync(null as SearchResult);

            Assert.Equal(SearchResult.Empty, await service.FindFirstAsync(query));
        }
    }
}
