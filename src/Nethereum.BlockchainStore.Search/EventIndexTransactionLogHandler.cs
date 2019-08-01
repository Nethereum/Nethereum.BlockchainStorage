//using Nethereum.BlockchainProcessing.Handlers;
//using Nethereum.Contracts;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;
//using Nethereum.BlockProcessing.ValueObjects;

//namespace Nethereum.BlockchainStore.Search
//{
//    public class EventIndexTransactionLogHandler<TEvent> : ITransactionLogHandler<TEvent>, IDisposable where TEvent : class, new()
//    {
//        private readonly IEventIndexer<TEvent> _indexer;
//        private readonly int _logsPerIndexBatch;
//        private readonly Queue<EventLog<TEvent>> _currentBatch = new Queue<EventLog<TEvent>>();

//        public EventIndexTransactionLogHandler(IEventIndexer<TEvent> indexer, int logsPerIndexBatch = 1)
//        {
//            _indexer = indexer;
//            _logsPerIndexBatch = logsPerIndexBatch;
//        }

//        public void Dispose()
//        {
//            if (_currentBatch.Any())
//            {
//                _indexer.IndexAsync(_currentBatch).Wait();
//                _currentBatch.Clear();
//            }
//        }

//        public int Pending => _currentBatch.Count;

//        public async Task HandleAsync(LogWithReceiptAndTransaction filterLogWithReceiptAndTransactionLog)
//        {
//            try
//            {
//                if(!filterLogWithReceiptAndTransactionLog.IsForEvent<TEvent>()) return;

//                var eventValues = filterLogWithReceiptAndTransactionLog.Decode<TEvent>();
//                if (eventValues == null)
//                {
//                    return;
//                }

//                _currentBatch.Enqueue(eventValues);
//                if (_currentBatch.Count == _logsPerIndexBatch)
//                {
//                    await _indexer.IndexAsync(_currentBatch);
//                    _currentBatch.Clear();
//                }
//            }
//            catch (Exception)
//            {
//                //Error whilst handling transaction log
//                //expected event signature may differ from the expected event.   
//            }
//        }
//    }
//}
