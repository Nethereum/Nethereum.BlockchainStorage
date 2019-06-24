using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.BlockchainProcessing.Queue.Azure.Processing.Logs;
using Nethereum.Contracts;
using Nethereum.LogProcessing;
using Nethereum.RPC.Eth.DTOs;
using System;
using System.Threading.Tasks;

namespace Nethereum.BlockchainProcessing.Processing.Logs
{
    public static class LogsProcessingExtensions
    {
        public static async Task<ILogsProcessorBuilder> AddToQueueAsync(
            this ILogsProcessorBuilder eventLogProcessor,
            AzureSubscriberQueueFactory queueFactory,
            string queueName,
            Predicate<FilterLog> predicate = null,
            Func<FilterLog, object> mapper = null)
        {
            var queue = await queueFactory.GetOrCreateQueueAsync(queueName);
            eventLogProcessor.AddToQueue(queue, predicate, mapper);
            return eventLogProcessor;
        }

        public static async Task<ILogsProcessorBuilder> AddToQueueAsync(
            this ILogsProcessorBuilder eventLogProcessor,
            string azureConnectionString,
            string queueName,
            Predicate<FilterLog> predicate = null,
            Func<FilterLog, object> mapper = null)
        {
            var factory = new AzureSubscriberQueueFactory(azureConnectionString);
            return await eventLogProcessor.AddToQueueAsync(factory, queueName, predicate, mapper);
        }

        public static async Task<ILogsProcessorBuilder> AddToQueueAsync<TEventDto>(
            this ILogsProcessorBuilder eventLogProcessor,
            AzureSubscriberQueueFactory queueFactory,
            string queueName,
            Predicate<EventLog<TEventDto>> predicate = null,
            Func<EventLog<TEventDto>, object> mapper = null)
            where TEventDto : class, IEventDTO, new()
        {
            var queue = await queueFactory.GetOrCreateQueueAsync(queueName);
            eventLogProcessor.AddToQueue(queue, predicate, mapper);
            return eventLogProcessor;
        }

        public static async Task<ILogsProcessorBuilder> AddToQueueAsync<TEventDto>(
            this ILogsProcessorBuilder eventLogProcessor,
            string azureConnectionString,
            string queueName,
            Predicate<EventLog<TEventDto>> predicate = null,
            Func<EventLog<TEventDto>, object> mapper = null)
            where TEventDto : class, IEventDTO, new()
        {
            var factory = new AzureSubscriberQueueFactory(azureConnectionString);
            return await eventLogProcessor.AddToQueueAsync(factory, queueName, predicate, mapper);
        }

    }
}
