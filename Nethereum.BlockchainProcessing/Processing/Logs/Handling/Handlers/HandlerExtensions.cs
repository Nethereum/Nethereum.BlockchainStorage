using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using Nethereum.Contracts;
using Nethereum.BlockchainProcessing.Processing.Logs.Handling.Handlers.Handlers;
using Nethereum.BlockchainProcessing.Handlers;

namespace Nethereum.BlockchainProcessing.Processing.Logs.Handling.Handlers
{
    public static class HandlerExtensions 
    {
        public static void AddStorageHandler(this EventSubscription subscription, ILogHandler logHandler, long id = 0)
        {
            subscription.AddHandler(new StorageHandler(subscription, id, logHandler));
        }

        public static void AddQueueHandler(
            this EventSubscription subscription, IQueue queue, IQueueMessageMapper mapper = null, long id = 0)
        {
            subscription.AddHandler(new QueueHandler(subscription, id, queue, mapper));
        }

        public static void AddQueueHandler(
            this EventSubscription subscription, IQueue queue, Func<DecodedEvent, object> mappingFunc, long id = 0)
        {
            AddQueueHandler(subscription, queue, new QueueMessageMapper(mappingFunc), id);
        }

        public static EventLogQueueMessage ToQueueMessage(this DecodedEvent decodedEvent)
        {
            var msg = new EventLogQueueMessage
            {
                Key = decodedEvent.Key,
                Event = decodedEvent.DecodedEventDto,
                State = decodedEvent.State,
                Transaction = decodedEvent.Transaction,
                Log = decodedEvent.Log,
                ParameterValues = decodedEvent.Event.Select(p => new EventParameterValue{
                    Order = p.Parameter.Order,
                    AbiType = p.Parameter.ABIType.Name,
                    Name = p.Parameter.Name,
                    Value = p.Result,
                    Indexed = p.Parameter.Indexed
                }).ToList()
            };

            return msg;
        }

        public static TEventDto GetDecodedEventDto<TEventDto>(this EventLogQueueMessage msg) where TEventDto : class, new()
        {
            if (msg.Event == null) return null;
            if (msg.Event is TEventDto dto) return dto;
            if (msg.Event is JObject jObject) return jObject.ToObject<TEventDto>();
            if (msg.Log != null) return msg.Log.DecodeEvent<TEventDto>()?.Event;

            return null;
        }
    }

}
