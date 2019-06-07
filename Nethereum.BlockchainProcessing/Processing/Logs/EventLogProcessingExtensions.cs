using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Nethereum.BlockchainProcessing.Processing.Logs
{
    public static class EventLogProcessingExtensions
    {
        public static void AddRange<T>(this ConcurrentBag<T> bag, IEnumerable<T> items)
        {
            foreach(var item in items) bag.Add(item);
        }

        /// <summary>
        /// Ensures the dependency will be disposed when the event log processor is disposed
        /// </summary>
        /// <param name="eventLogProcessor"></param>
        /// <param name="dependency"></param>
        public static void MarkForDisposal(this IEventLogProcessor eventLogProcessor, IDisposable dependency)
        {
            eventLogProcessor.OnDisposing += disposeHandler;

            void disposeHandler(object s, EventArgs src)
            {
                dependency.Dispose();
                eventLogProcessor.OnDisposing -= disposeHandler;
            }
        }

        /// <summary>
        /// Ensures the dependency will be disposed when the event log processor is disposed
        /// </summary>
        /// <param name="eventLogProcessor"></param>
        /// <param name="dependency"></param>
        public static void MarkForDisposal(this BlockchainBatchProcessorService eventLogProcessor, IDisposable dependency)
        {
            eventLogProcessor.OnDisposing += disposeHandler;

            void disposeHandler(object s, EventArgs src)
            {
                dependency.Dispose();
                eventLogProcessor.OnDisposing -= disposeHandler;
            }
        }
    }
}
