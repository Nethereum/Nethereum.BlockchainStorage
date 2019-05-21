using Nethereum.BlockchainProcessing.Queue.Azure.Processing.Logs;
using Nethereum.Contracts;
using Nethereum.RPC.Eth.DTOs;
using System;
using System.Threading.Tasks;

namespace Nethereum.BlockchainProcessing.Processing.Logs
{
    public static class EventLogProcessingExtensions
    {
        public static async Task<IEventLogProcessor> CatchAllAndQueueAsync(
            this IEventLogProcessor eventLogProcessor,
            AzureSubscriberQueueFactory queueFactory,
            string queueName,
            Predicate<FilterLog> predicate = null,
            Func<FilterLog, object> mapper = null)
        {
            var queue = await queueFactory.GetOrCreateQueueAsync(queueName);
            eventLogProcessor.CatchAllAndQueue(queue, predicate, mapper);
            return eventLogProcessor;
        }

        public static async Task<IEventLogProcessor> CatchAllAndQueueAsync(
            this IEventLogProcessor eventLogProcessor,
            string azureConnectionString,
            string queueName,
            Predicate<FilterLog> predicate = null,
            Func<FilterLog, object> mapper = null)
        {
            var factory = new AzureSubscriberQueueFactory(azureConnectionString);
            return await eventLogProcessor.CatchAllAndQueueAsync(factory, queueName, predicate, mapper);
        }

        public static async Task<IEventLogProcessor> SubscribeAndQueueAsync<TEventDto>(
            this IEventLogProcessor eventLogProcessor, 
            AzureSubscriberQueueFactory queueFactory, 
            string queueName,
            Predicate<EventLog<TEventDto>> predicate = null,
            Func<EventLog<TEventDto>, object> mapper = null)
            where TEventDto : class, new()
        {
            var queue = await queueFactory.GetOrCreateQueueAsync(queueName);
            eventLogProcessor.SubscribeAndQueue(queue, predicate, mapper);
            return eventLogProcessor;
        }

        public static async Task<IEventLogProcessor> SubscribeAndQueueAsync<TEventDto>(
            this IEventLogProcessor eventLogProcessor, 
            string azureConnectionString, 
            string queueName,
            Predicate<EventLog<TEventDto>> predicate = null,
            Func<EventLog<TEventDto>, object> mapper = null)
            where TEventDto : class, new()
        {
            var factory = new AzureSubscriberQueueFactory(azureConnectionString);
            return await eventLogProcessor.SubscribeAndQueueAsync(factory, queueName, predicate, mapper);
        }


    }
}
