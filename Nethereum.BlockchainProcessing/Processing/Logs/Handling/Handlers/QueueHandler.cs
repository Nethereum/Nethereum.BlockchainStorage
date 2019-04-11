using Nethereum.RPC.Eth.DTOs;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nethereum.BlockchainProcessing.Processing.Logs.Handling.Handlers
{
    public class QueueHandler : EventHandlerBase, IEventHandler
    {
        public QueueHandler(IEventSubscription subscription, long id, IQueue queue)
            :base(subscription, id)
        {
            Queue = queue;
        }

        public IQueue Queue { get; }

        public async Task<bool> HandleAsync(DecodedEvent decodedEvent)
        {
            var msgToQueue = decodedEvent.ToQueueMessage();
            await Queue.AddMessageAsync(msgToQueue);
            return true;
        }
    }

    public static class QueueMessageExtensions 
    {
        public static void AddQueueHandler(this EventSubscription subscription, IQueue queue, long id = 0)
        {
            subscription.AddHandler(new QueueHandler(subscription, id, queue));
        }

        public static EventLogQueueMessage ToQueueMessage(this DecodedEvent decodedEvent)
        {
            var msg = new EventLogQueueMessage
            {
                Key = decodedEvent.Key,
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
    }


    public class EventLogQueueMessage
    {
        public string Key { get; set; }

        public Dictionary<string, object> State { get; set; }

        public Transaction Transaction { get; set; }

        public List<EventParameterValue> ParameterValues { get;set;}

        public FilterLog Log { get;set;}

    }

    public class EventParameterValue
    {
        public int Order { get;set;}
        public string Name { get;set;}
        public string AbiType { get; set;}
        public object Value { get;set;}

        public bool Indexed { get;set;}
    }
}
