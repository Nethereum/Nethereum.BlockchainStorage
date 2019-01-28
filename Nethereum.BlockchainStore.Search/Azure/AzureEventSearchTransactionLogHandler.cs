using Nethereum.BlockchainProcessing.Handlers;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;
using Nethereum.Contracts;

namespace Nethereum.BlockchainStore.Search.Azure
{


    public class AzureEventSearchTransactionLogHandler<TEvent> : ITransactionLogHandler<TEvent>, IDisposable where TEvent : class, new()
    {
        private readonly IEventSearchIndexer<TEvent> _indexer;
        private readonly int _logsPerIndexBatch;
        private ConcurrentQueue<EventLog<TEvent>> _currentBatch = new ConcurrentQueue<EventLog<TEvent>>();

        public AzureEventSearchTransactionLogHandler(IEventSearchIndexer<TEvent> indexer, int logsPerIndexBatch = 1)
        {
            _indexer = indexer;
            this._logsPerIndexBatch = logsPerIndexBatch;
        }

        public void Dispose()
        {
            if (!_currentBatch.IsEmpty)
            {
                _indexer.IndexAsync(_currentBatch).Wait();
            }
        }

        public int Pending => _currentBatch.Count;

        public async Task HandleAsync(TransactionLogWrapper transactionLog)
        {
            try
            {
                if(!transactionLog.IsForEvent<TEvent>()) return;

                var eventValues = transactionLog.Decode<TEvent>();
                _currentBatch.Enqueue(eventValues);
                if (_currentBatch.Count == _logsPerIndexBatch)
                {
                    await _indexer.IndexAsync(_currentBatch);
                    _currentBatch = new ConcurrentQueue<EventLog<TEvent>>();
                }
            }
            catch (Exception x)
            {
                //Error whilst handling transaction log
                //expected event signature may differ from the expected event.   
            }
        }
    }
}
