using System.Collections.Concurrent;

namespace Nethereum.BlockchainStore.Search
{
    internal static class ConcurrentQueueExtensions
    {
        public static void Clear<T>(this ConcurrentQueue<T> queue)
        {
            while (queue.TryDequeue(out _))
            {
                // do nothing
            }
        }
    }
}
