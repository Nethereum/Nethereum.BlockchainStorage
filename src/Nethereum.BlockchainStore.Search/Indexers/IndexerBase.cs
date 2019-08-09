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

        protected IndexerBase(Func<TSource, TSearchDocument> mapper, int documentsPerBatch = 1)
        {
            _documentsPerIndexBatch = documentsPerBatch;
            this.mapper = mapper;
        }

        protected void AddToBatch(TSource eventLog)
        {
            var searchDoc = mapper.Invoke(eventLog);

            if(searchDoc == null) return;

            CurrentBatch.Enqueue(searchDoc);
        }

        protected bool IsReadyToSend()
        {
            return CurrentBatch.Count >= _documentsPerIndexBatch;
        }

        public int PendingDocumentCount => CurrentBatch.Count;

        public async Task IndexPendingDocumentsAsync()
        {
            if(CurrentBatch.IsEmpty) return;
            await LockAndSend().ConfigureAwait(false);
        }

        public int Indexed {get; private set; }

        public ConcurrentQueue<TSearchDocument> CurrentBatch { get; } = new ConcurrentQueue<TSearchDocument>();

        public virtual void Dispose()
        {
            IndexPendingDocumentsAsync().Wait();
        }

        public abstract Task<long> DocumentCountAsync();

        public virtual async Task IndexAsync(TSource source)
        {
            AddToBatch(source);
            if (IsReadyToSend())
            {
                await LockAndSend().ConfigureAwait(false);
            }
        }

        private async Task LockAndSend()
        {
            await _semaphoreSlim.WaitAsync();
            try
            { 
                var numberToSend = CurrentBatch.Count;
                await SendBatchAsync(CurrentBatch);
                Indexed += numberToSend;
                CurrentBatch.Clear();
            }
            finally
            {
                _semaphoreSlim.Release();
            }
        }

        protected abstract Task SendBatchAsync(IEnumerable<TSearchDocument> docs);
    }
}
