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
    public class EventHandlerCoordinator : IEventHandlerCoordinator
    {
        public EventHandlerCoordinator(long subscriberId = 0, long eventSubscriptionId = 0, IEnumerable<IEventHandler> handlers = null)
        {
            SubscriberId = subscriberId;
            EventSubscriptionId = eventSubscriptionId;
            Handlers = handlers ?? Array.Empty<IEventHandler>();
        }

        public long SubscriberId { get; }
        public long EventSubscriptionId { get; }
        public IEnumerable<IEventHandler> Handlers { get; }

        public async Task HandleAsync(EventABI abi, params FilterLog[] eventLogs)
        {
            foreach(var log in eventLogs)
            {
                DecodedEvent decodedEvent = null;
                try
                {
                    decodedEvent = log.ToDecodedEvent(abi);
                }
                catch(Exception x)
                {
                    if(x.Message.StartsWith("Number of indexes don't match the number of topics"))
                    {
                        return;
                    }
                    throw;
                }

                decodedEvent.State["SubscriberId"] = SubscriberId;
                decodedEvent.State["EventSubscriptionId"] = EventSubscriptionId;

                foreach (var handler in Handlers)
                {
                    decodedEvent.State["HandlerInvocations"] = 1 + (int)decodedEvent.State["HandlerInvocations"];

                    if (!await handler.HandleAsync(decodedEvent))
                    {
                        break;
                    }
                }
            }
        }


    }
}
