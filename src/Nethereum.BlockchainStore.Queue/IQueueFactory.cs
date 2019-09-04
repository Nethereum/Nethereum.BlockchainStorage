using System;
using System.Threading.Tasks;

namespace Nethereum.BlockchainStore.Queue
{
    public interface IQueueFactory
    {
        Task ClearQueueAsync(string queueName);
        Task DeleteQueueAsync(string queueName);
        Task<IQueue> GetOrCreateQueueAsync(string queueName);
        Task<IQueue> GetOrCreateQueueAsync<TSource, TQueueMessage>(string queueName, Func<TSource, TQueueMessage> mapper)
            where TSource : class
            where TQueueMessage : class;
    }
}