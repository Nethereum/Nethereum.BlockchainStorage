using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Nethereum.BlockchainStore.Search.Tests.Indexers
{
    public class IndexerBaseTests
    {
        public class Source { }

        public class SearchDocument { }
        public class TestIndexer : IndexerBase<Source, SearchDocument>
        {
            public TestIndexer(Func<Source, SearchDocument> mapper, int documentsPerIndexBatch = 1) : base(mapper, documentsPerIndexBatch)
            {
            }

            public List<(DocumentIndexAction action, SearchDocument document)> IndexedDocuments { get; } = new List<(DocumentIndexAction action, SearchDocument document)>();

            public override Task<long> DocumentCountAsync()
            {
                throw new NotImplementedException();
            }

            protected override Task SendBatchAsync(IEnumerable<(DocumentIndexAction action, SearchDocument document)> docs)
            {
                IndexedDocuments.AddRange(docs);
                return Task.CompletedTask;
            }
        }

        [Fact]
        public async Task InvokesMapper()
        {
            var source = new Source();
            var searchDoc = new SearchDocument();

            var indexer = new TestIndexer((s) => searchDoc);

            await indexer.IndexAsync(source);

            Assert.Single(indexer.IndexedDocuments, (DocumentIndexAction.uploadOrMerge, searchDoc));
        }

        [Fact]
        public async Task WillNotSendANullSearchDocument()
        {
            var source = new Source();
            SearchDocument nullSearchDoc = null;

            // simulate the mapper returning a null
            var indexer = new TestIndexer((s) => nullSearchDoc);

            await indexer.IndexAsync(source);

            Assert.Empty(indexer.IndexedDocuments);
        }

        [Fact]
        public async Task IsThreadSafe()
        {
            const int DOCUMENTS_TO_INDEX = 100;

            var indexer = new TestIndexer((s) => new SearchDocument());

            foreach(var i in Enumerable.Range(0, DOCUMENTS_TO_INDEX).AsParallel())
            {
                await indexer.IndexAsync(new Source());
            }

            Assert.Equal(DOCUMENTS_TO_INDEX, indexer.IndexedDocuments.Count);
        }

        [Fact]
        public async Task BatchHandling()
        {
            var source = new Source();
            var searchDoc = new SearchDocument();

            var indexer = new TestIndexer((s) => searchDoc, documentsPerIndexBatch: 2);

            //add doc to batch - too small to process
            await indexer.IndexAsync(source);
            Assert.Empty(indexer.IndexedDocuments);
            Assert.Equal(1, indexer.PendingDocumentCount);

            //add another doc - batch is big enough to trigger processing
            await indexer.IndexAsync(source);
            Assert.Equal(2, indexer.IndexedDocuments.Count);
            Assert.Equal(0, indexer.PendingDocumentCount);

            //add another doc - batch is too small for processing
            await indexer.IndexAsync(source);
            Assert.Equal(2, indexer.IndexedDocuments.Count);
            Assert.Equal(1, indexer.PendingDocumentCount);

            //force processing of pending docs
            await indexer.IndexPendingDocumentsAsync();
            Assert.Equal(3, indexer.IndexedDocuments.Count);
            Assert.Equal(0, indexer.PendingDocumentCount);
        }

        [Fact]
        public async Task SubmitsPendingDocumentsOnDispose()
        {
            var source = new Source();
            var searchDoc = new SearchDocument();

            var indexer = new TestIndexer((s) => searchDoc, documentsPerIndexBatch: 2);

            //add doc to batch - too small to process
            await indexer.IndexAsync(source);
            Assert.Empty(indexer.IndexedDocuments);
            Assert.Equal(1, indexer.PendingDocumentCount);

            indexer.Dispose();
            Assert.Single(indexer.IndexedDocuments);
            Assert.Equal(0, indexer.PendingDocumentCount);

        }

    }
}
