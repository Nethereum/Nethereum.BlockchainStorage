//using Nethereum.BlockchainProcessing.Processing.Logs;
//using Nethereum.Contracts;
//using Nethereum.Contracts.Extensions;
//using Nethereum.RPC.Eth.DTOs;
//using System;
//using System.Collections.Concurrent;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;

//namespace Nethereum.BlockchainStore.Search
//{
//    public interface IEventIndexProcessor<TEvent> : ILogProcessor, IDisposable where TEvent : class, new()
//    {
//        int Pending { get; }
//        IEventIndexer<TEvent> Indexer { get; }
//    }

//    public class EventIndexProcessor<TEvent> : IEventIndexProcessor<TEvent> where TEvent : class, new()
//    {
//        private readonly int _logsPerIndexBatch;
//        private readonly ConcurrentQueue<EventLog<TEvent>> _currentBatch = new ConcurrentQueue<EventLog<TEvent>>();
//        private readonly IEventFunctionProcessor _functionProcessor;

//        public EventIndexProcessor(
//            IEventIndexer<TEvent> indexer,
//            IEventFunctionProcessor functionProcessor = null,
//            int logsPerIndexBatch = 1,
//            Predicate<EventLog<TEvent>> predicate = null)
//        {
//            Indexer = indexer;
//            _functionProcessor = functionProcessor;
//            _logsPerIndexBatch = logsPerIndexBatch;
//            Predicate = predicate ?? new Predicate<EventLog<TEvent>>((l) => true);
//        }

//        public void Dispose()
//        {
//            if (!_currentBatch.IsEmpty)
//            {
//                Indexer.IndexAsync(_currentBatch);
//                _currentBatch.Clear();
//            }
//        }

//        public int Pending => _currentBatch.Count;

//        public IEventIndexer<TEvent> Indexer { get; }
//        public Predicate<EventLog<TEvent>> Predicate { get; }

//        public Task<bool> IsLogForMeAsync(FilterLog log)
//        {
//            return Task.FromResult(log.IsLogForEvent<TEvent>());
//        }

//        public async Task ProcessLogsAsync(params FilterLog[] eventLogs)
//        {
//            var decoded = DecodeInToBatch(eventLogs);

//            while (_currentBatch.Count >= _logsPerIndexBatch)
//            {
//                List<EventLog<TEvent>> logsToSend = GetLogsToIndex();

//                await Indexer.IndexAsync(logsToSend);
//            }

//            await ProcessFunctions(decoded);
//        }

//        private async Task ProcessFunctions(EventLog<TEvent>[] eventLogs)
//        {
//            if (_functionProcessor == null) return;

//            await _functionProcessor.ProcessAsync(eventLogs);
//        }

//        private List<EventLog<TEvent>> GetLogsToIndex()
//        {
//            var logsToSend = new List<EventLog<TEvent>>();
//            for (int i = 0; i < _logsPerIndexBatch; i++)
//            {
//                if (_currentBatch.TryDequeue(out EventLog<TEvent> log))
//                {
//                    logsToSend.Add(log);
//                }
//            }

//            return logsToSend;
//        }

//        private EventLog<TEvent>[] DecodeInToBatch(FilterLog[] eventLogs)
//        {
//            var decoded = TryDecode(eventLogs).ToArray();

//            foreach (var decodedLog in decoded.Where(d => Predicate(d)))
//            {
//                _currentBatch.Enqueue(decodedLog);
//            }

//            return decoded;
//        }

//        private IEnumerable<EventLog<TEvent>> TryDecode(FilterLog[] eventLogs)
//        {
//            var decoded = new List<EventLog<TEvent>>();

//            foreach (var log in eventLogs)
//            {
//                try
//                {
//                    var decodedEvent = log.DecodeEvent<TEvent>();
//                    if (decodedEvent != null)
//                    {
//                        decoded.Add(decodedEvent);
//                    }
//                }
//                catch
//                {
//                    //ignore
//                }
//            }

//            return decoded;
//        }
//    }
//}
