using Nethereum.ABI.Model;
using Nethereum.RPC.Eth.DTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace Nethereum.BlockchainProcessing.Processing.Logs.Handling
{
    public class EventHandlerCoordinator : IEventHandlerCoordinator
    {
        public EventHandlerCoordinator(
            IEventHandlerHistoryDb eventHandlerHistory, 
            long subscriberId = 0, 
            long eventSubscriptionId = 0, 
            IEnumerable<IEventHandler> handlers = null)
        {
            History = eventHandlerHistory;
            SubscriberId = subscriberId;
            EventSubscriptionId = eventSubscriptionId;
            Handlers = handlers ?? Array.Empty<IEventHandler>();
        }

        public IEventHandlerHistoryDb History { get; }
        public long SubscriberId { get; }
        public long EventSubscriptionId { get; }
        public IEnumerable<IEventHandler> Handlers { get; }

        public async Task HandleAsync(EventABI abi, params FilterLog[] eventLogs)
        {
            foreach(var log in eventLogs)
            {
                if (!TryDecode(abi, log, out DecodedEvent decodedEvent))
                {
                    continue;
                }

                SetStateValues(decodedEvent);

                await InvokeHandlers(decodedEvent);
            }
        }

        private async Task InvokeHandlers(DecodedEvent decodedEvent)
        {
            foreach (var handler in Handlers)
            {
                if (await History.ContainsEventHandlerHistory(handler.Id, decodedEvent.Key))
                {
                    continue;
                }

                decodedEvent.State["HandlerInvocations"] = 1 + (int)decodedEvent.State["HandlerInvocations"];

                var invokeNextHandler = await handler.HandleAsync(decodedEvent);

                await History.AddEventHandlerHistory(handler.Id, decodedEvent.Key);

                if (!invokeNextHandler)
                {
                    break;
                }
            }
        }

        private void SetStateValues(DecodedEvent decodedEvent)
        {
            decodedEvent.State["SubscriberId"] = SubscriberId;
            decodedEvent.State["EventSubscriptionId"] = EventSubscriptionId;
        }

        private bool TryDecode(EventABI abi, FilterLog log, out DecodedEvent decodedEvent)
        {
            decodedEvent = null;
            try
            {
                decodedEvent = log.ToDecodedEvent(abi);
                return true;
            }
            catch (Exception x) when (x.Message.StartsWith("Number of indexes don't match the number of topics"))
            {
                return false;
            }
        }


    }
}
