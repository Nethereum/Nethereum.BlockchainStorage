using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;
using Microsoft.Rest.Azure;
using Moq;
using Nethereum.BlockchainStore.Search.Azure;
using Nethereum.Contracts;
using Nethereum.RPC.Eth.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Index = Microsoft.Azure.Search.Models.Index;

namespace Nethereum.BlockchainStore.Search.Tests.Azure
{
    public class AzureEventLogIndexingTests
    {
        Mock<IAzureSearchService> _mockAzureSearchService;
        Mock<ISearchIndexClient> _mockSearchIndexClient;
        Mock<IDocumentsOperations> _mockDocumentOperations;
        IAzureSearchService _azureSearchService;
        IndexBatch<Dictionary<string, object>> _indexedBatch;

        public AzureEventLogIndexingTests()
        {
            _mockAzureSearchService = new Mock<IAzureSearchService>();
            _azureSearchService = _mockAzureSearchService.Object;
            _mockAzureSearchService.Setup(s => s.CreateIndexAsync(It.IsAny<Index>())).Returns<Index>((i) => Task.FromResult(i));
            _mockSearchIndexClient = new Mock<ISearchIndexClient>();
            _mockAzureSearchService.Setup(s => s.GetOrCreateIndexClient(It.IsAny<string>())).Returns(_mockSearchIndexClient.Object);

            _mockDocumentOperations = new Mock<IDocumentsOperations>();

            _mockSearchIndexClient.Setup(c => c.Documents).Returns(_mockDocumentOperations.Object);

            MockIndexClient<Dictionary<string, object>>(
                _mockDocumentOperations, (indexedBatch, _) => _indexedBatch = indexedBatch);
        }

        static void MockIndexClient<TDocument>(
            Mock<IDocumentsOperations> _mockDocumentOperations, 
            Action<IndexBatch<TDocument>, SearchRequestOptions> callBack) where TDocument : class   
        {
            var _azureOperationResponse = new AzureOperationResponse<DocumentIndexResult>();

            _mockDocumentOperations.Setup(o => o.IndexWithHttpMessagesAsync(
                    It.IsAny<IndexBatch<TDocument>>(),
                    It.IsAny<SearchRequestOptions>(),
                    It.IsAny<Dictionary<string, List<string>>>(),
                    It.IsAny<CancellationToken>()))
                .Callback<
                    IndexBatch<TDocument>,
                    SearchRequestOptions,
                    Dictionary<string, List<string>>,
                    CancellationToken>((indexBatch, searchRequestOptions, customHeaders, ctx) =>
                    {
                        callBack(indexBatch, searchRequestOptions);
                    })
                .ReturnsAsync(_azureOperationResponse);
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

            var filterLog = new FilterLog {
                BlockNumber = new Hex.HexTypes.HexBigInteger(4309), 
                TransactionHash = "0x3C81039C578811A85742F2476DA90E363A88CA93763DB4A194E35367D9A72FD8", 
                LogIndex = new Hex.HexTypes.HexBigInteger(9) };

            await processor.ExecuteAsync(filterLog);
            
            Assert.Single(_indexedBatch.Actions);
            var action = _indexedBatch.Actions.First();
            Assert.Equal(IndexActionType.MergeOrUpload, action.ActionType);
            Assert.Equal(filterLog.Key(), action.Document[PresetSearchFieldName.log_key.ToString()]);
        }
    }
}
