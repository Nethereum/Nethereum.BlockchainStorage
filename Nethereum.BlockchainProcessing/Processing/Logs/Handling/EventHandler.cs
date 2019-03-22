using Nethereum.ABI.Model;
using Nethereum.RPC.Eth.DTOs;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Nethereum.BlockchainProcessing.Processing.Logs.Handling
{
    public class EventHandler : IEventHandler
    {
        public EventHandler(long subscriberId = 0, long eventSubscriptionId = 0, IEnumerable<IDecodedEventHandler> handlers = null)
        {
            SubscriberId = subscriberId;
            EventSubscriptionId = eventSubscriptionId;
            Handlers = handlers ?? Array.Empty<IDecodedEventHandler>();
        }

        public long SubscriberId { get; }
        public long EventSubscriptionId { get; }
        public IEnumerable<IDecodedEventHandler> Handlers { get; }

        public async Task HandleAsync(EventABI abi, params FilterLog[] eventLogs)
        {
            foreach(var log in eventLogs)
            {
                var decodedEvent = log.ToDecodedEvent(abi);

                decodedEvent.Metadata["SubscriberId"] = SubscriberId;
                decodedEvent.Metadata["EventSubscriptionId"] = EventSubscriptionId;
                decodedEvent.Metadata["EventAbiName"] = abi?.Name;

                Debug.Write(JsonConvert.SerializeObject(decodedEvent));
                Debug.WriteLine("");

                foreach(var handler in Handlers)
                {
                    if (!await handler.HandleAsync(decodedEvent))
                    {
                        break;
                    }
                }
            }
        }
    }
}
