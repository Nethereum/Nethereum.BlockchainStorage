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
    }
}
