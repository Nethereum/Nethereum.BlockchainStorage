using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Nethereum.BlockchainStore.Search
{
    public abstract class IndexerBase<TSource, TSearchDocument> where TSearchDocument : class
    {
        private readonly int _logsPerIndexBatch;
        private readonly Func<TSource, TSearchDocument> mapper;
        private readonly SemaphoreSlim _semaphoreSlim = new SemaphoreSlim(1);

        protected IndexerBase(Func<TSource, TSearchDocument> mapper, int logsPerIndexBatch = 1)
        {
            _logsPerIndexBatch = logsPerIndexBatch;
            this.mapper = mapper;
        }

        protected void AddToBatch(TSource eventLog)
        {
            var searchDoc = mapper.Invoke(eventLog);
            CurrentBatch.Enqueue(searchDoc);
        }

        protected bool IsReadyToSend()
        {
            return CurrentBatch.Count >= _logsPerIndexBatch;
        }

        public int Pending => CurrentBatch.Count;

        public int Indexed {get; private set; }

        public ConcurrentQueue<TSearchDocument> CurrentBatch { get; } = new ConcurrentQueue<TSearchDocument>();

        public virtual void Dispose()
        {
            if(!CurrentBatch.IsEmpty)
            {
                LockAndSend().Wait();
            }
        }

        public abstract Task<long> DocumentCountAsync();

        public virtual async Task IndexAsync(TSource log)
        {
            AddToBatch(log);
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
