using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;
using Moq;
using Nethereum.BlockchainStore.Search.Azure;
using Nethereum.RPC.Eth.DTOs;
using System.Threading.Tasks;
using Xunit;

namespace Nethereum.BlockchainStore.Search.Tests.Azure
{
    public class AzureEventLogIndexingTests
    {
        Mock<IAzureSearchService> _mockAzureSearchService;
        Mock<ISearchIndexClient> _mockSearchIndexClient;
        IAzureSearchService _azureSearchService;

        public AzureEventLogIndexingTests()
        {
            _mockAzureSearchService = new Mock<IAzureSearchService>();
            _azureSearchService = _mockAzureSearchService.Object;
            _mockAzureSearchService.Setup(s => s.CreateIndexAsync(It.IsAny<Index>())).Returns<Index>((i) => Task.FromResult(i));
            _mockSearchIndexClient = new Mock<ISearchIndexClient>();
            _mockAzureSearchService.Setup(s => s.GetOrCreateIndexClient(It.IsAny<string>())).Returns(_mockSearchIndexClient.Object);
        }

        [Fact]
        public async Task FilterLogIndexing()
        {
            //create the index
            var index = FilterLogIndexUtil.Create("my-logs");
            await _azureSearchService.CreateIndexAsync(index);

            //get the index client
            var indexClient = _azureSearchService.GetOrCreateIndexClient(index.Name);

            var indexer = new AzureFilterLogIndexer(index, indexClient);
            var processor = new FilterLogProcessor(indexer);

            var filterLog = new FilterLog();

            await processor.ExecuteAsync(filterLog);
            

        }
    }
}
