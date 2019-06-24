using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.BlockchainProcessing.Processing.Logs;
using Nethereum.BlockchainProcessing.Processing.Logs.Handling;
using Nethereum.Contracts;
using Nethereum.RPC.Eth.DTOs;
using System;

namespace Nethereum.LogProcessing
{
    public static class LogsProcessorBuilderExtensions
    {
        public static ILogsProcessorBuilder AddToQueue(this ILogsProcessorBuilder builder, IQueue queue, Predicate<FilterLog> predicate = null, Func<FilterLog, object> mapper = null)
        {
            builder.Processors.Add(new EventLogQueueProcessor(queue, predicate, mapper));
            return builder;
        }
        public static ILogsProcessorBuilder AddToQueue<TEventDto>(this ILogsProcessorBuilder builder, IQueue queue, Predicate<EventLog<TEventDto>> predicate = null, Func<EventLog<TEventDto>, object> mapper = null) where TEventDto : class, IEventDTO, new()
        {
            builder.Processors.Add(new EventLogQueueProcessor<TEventDto>(queue, predicate, mapper));
            return builder;
        }
        public static ILogsProcessorBuilder Add(this ILogsProcessorBuilder builder, EventSubscription eventSubscription)
        {
            builder.Processors.Add(eventSubscription);
            return builder;
        }

        public static ILogsProcessorBuilder Add<TEventDto>(ILogsProcessorBuilder builder, EventSubscription<TEventDto> eventSubscription) where TEventDto : class, new()
        {
            builder.Processors.Add(eventSubscription);
            return builder;
        }
    }
}
