using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Nethereum.BlockchainStore.Search
{

    public abstract class IndexerBase<TSource, TSearchDocument> where TSearchDocument : class
    {
        private readonly int _documentsPerIndexBatch;
        private readonly Func<TSource, TSearchDocument> mapper;
        private readonly SemaphoreSlim _semaphoreSlim = new SemaphoreSlim(1);

        public ConcurrentQueue<(DocumentIndexAction, TSearchDocument)> CurrentBatch { get; } = new ConcurrentQueue<(DocumentIndexAction, TSearchDocument)>();

        protected IndexerBase(Func<TSource, TSearchDocument> mapper, int documentsPerBatch = 1)
        {
            _documentsPerIndexBatch = documentsPerBatch;
            this.mapper = mapper;
        }

        protected virtual void AddToBatch(DocumentIndexAction indexAction, TSource eventLog)
        {
            var searchDoc = mapper.Invoke(eventLog);

            if(searchDoc == null) return;

            CurrentBatch.Enqueue((indexAction, searchDoc));
        }

        protected bool IsReadyToSend()
        {
            return CurrentBatch.Count >= _documentsPerIndexBatch;
        }

        public int PendingDocumentCount => CurrentBatch.Count;

        public async Task IndexPendingDocumentsAsync()
        {
            if(CurrentBatch?.IsEmpty ?? true) return;
            await SendCurrentBatch().ConfigureAwait(false);
        }

        public int Indexed {get; private set; }


        public virtual void Dispose()
        {
            _semaphoreSlim.WaitAsync().Wait();
            try
            {
                IndexPendingDocumentsAsync().Wait();
            }
            finally
            {
                _semaphoreSlim.Release();
            }
        }

        public abstract Task<long> DocumentCountAsync();

        public virtual async Task IndexAsync(TSource source, DocumentIndexAction indexAction = DocumentIndexAction.uploadOrMerge)
        {
            await _semaphoreSlim.WaitAsync();
            try
            {
                AddToBatch(indexAction, source);
                if (IsReadyToSend())
                {
                    await SendCurrentBatch().ConfigureAwait(false);
                }
            }
            finally
            {
                _semaphoreSlim.Release();
            }
        }

        private async Task SendCurrentBatch()
        {
            if(CurrentBatch.IsEmpty) return;
            var numberToSend = CurrentBatch.Count;
            await SendBatchAsync(CurrentBatch.ToArray());
            Indexed += numberToSend;
            CurrentBatch.Clear();
        }

        protected abstract Task SendBatchAsync(IEnumerable<(DocumentIndexAction action, TSearchDocument document)> docs);
    }
}
