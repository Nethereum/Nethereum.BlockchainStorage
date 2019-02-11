using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using Nethereum.BlockchainProcessing.Processing.Logs;
using Nethereum.Contracts;
using Nethereum.Contracts.Extensions;
using Nethereum.RPC.Eth.DTOs;

namespace Nethereum.BlockchainStore.Search
{
    public class EventIndexProcessor<TEvent> : ILogProcessor, IDisposable where TEvent : class, new()
    {
        private readonly IEventIndexer<TEvent> _indexer;
        private readonly int _logsPerIndexBatch;
        private readonly ConcurrentQueue<EventLog<TEvent>> _currentBatch = new ConcurrentQueue<EventLog<TEvent>>();

        public EventIndexProcessor(IEventIndexer<TEvent> indexer, int logsPerIndexBatch = 1)
        {
            _indexer = indexer;
            _logsPerIndexBatch = logsPerIndexBatch;
        }

        public void Dispose()
        {
            if (!_currentBatch.IsEmpty)
            {
                _indexer.IndexAsync(_currentBatch);
                _currentBatch.Clear();
            }
        }

        public int Pending => _currentBatch.Count;

        public bool IsLogForEvent(FilterLog log)
        {
            return log.IsLogForEvent<TEvent>();
        }

        public async Task ProcessLogsAsync(params FilterLog[] eventLogs)
        {
            DecodeInToBatch(eventLogs);

            while (_currentBatch.Count >= _logsPerIndexBatch)
            {
                List<EventLog<TEvent>> logsToSend = GetLogsToIndex();

                await _indexer.IndexAsync(logsToSend);
            }
        }

        private List<EventLog<TEvent>> GetLogsToIndex()
        {
            var logsToSend = new List<EventLog<TEvent>>();
            for (int i = 0; i < _logsPerIndexBatch; i++)
            {
                if (_currentBatch.TryDequeue(out EventLog<TEvent> log))
                {
                    logsToSend.Add(log);
                }
            }

            return logsToSend;
        }

        private void DecodeInToBatch(FilterLog[] eventLogs)
        {
            var decoded = TryDecode(eventLogs);
            foreach (var decodedLog in decoded)
            {
                _currentBatch.Enqueue(decodedLog);
            }
        }

        private IEnumerable<EventLog<TEvent>> TryDecode(FilterLog[] eventLogs)
        {
            var decoded = new List<EventLog<TEvent>>();

            foreach (var log in eventLogs)
            {
                try
                {
                    var decodedEvent = log.DecodeEvent<TEvent>();
                    if (decodedEvent != null)
                    {
                        decoded.Add(decodedEvent);
                    }
                }
                catch
                {
                    //ignore
                }
            }

            return decoded;
        }
    }


    internal static class ConcurrentQueueExtensions
    {
        public static void Clear<T>(this ConcurrentQueue<T> queue)
        {
            T item;
            while (queue.TryDequeue(out item))
            {
                // do nothing
            }
        }
    }
}
