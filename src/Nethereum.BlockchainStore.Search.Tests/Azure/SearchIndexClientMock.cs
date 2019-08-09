using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;
using Microsoft.Rest.Azure;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Nethereum.BlockchainStore.Search.Tests.Azure
{
    public class SearchIndexClientMock<TSearchDocument> where TSearchDocument : class
    {
        Mock<ISearchIndexClient> _mockSearchIndexClient;
        Mock<IDocumentsOperations> _mockDocumentOperations;

        public SearchIndexClientMock()
        {
            _mockSearchIndexClient = new Mock<ISearchIndexClient>();
            _mockDocumentOperations = new Mock<IDocumentsOperations>();
            _mockSearchIndexClient.Setup(c => c.Documents).Returns(_mockDocumentOperations.Object);

            IndexedBatches = new List<IndexBatch<TSearchDocument>>();

            MockIndexClient<TSearchDocument>(
                _mockDocumentOperations, (indexedBatch, _) => IndexedBatches.Add(indexedBatch));
        }

        public List<IndexBatch<TSearchDocument>> IndexedBatches {get; }
        public ISearchIndexClient SearchIndexClient => _mockSearchIndexClient.Object;

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

    }
}
