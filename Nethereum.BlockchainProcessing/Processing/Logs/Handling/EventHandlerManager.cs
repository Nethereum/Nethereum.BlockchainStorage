using Nethereum.ABI.Model;
using Nethereum.Contracts;
using Nethereum.RPC.Eth.DTOs;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Nethereum.BlockchainProcessing.Processing.Logs.Handling
{
    public class EventHandlerManager : IEventHandlerManager
    {
        public EventHandlerManager(
            IEventHandlerHistoryRepository eventHandlerHistory = null)
        {
            History = eventHandlerHistory;
        }

        public IEventHandlerHistoryRepository History { get; }

        public async Task HandleAsync(IEventSubscription subscription, EventABI[] abis, params FilterLog[] eventLogs)
        {
            foreach(var log in eventLogs)
            {
                if (!TryDecode(abis, log, out DecodedEvent decodedEvent))
                {
                    continue;
                }

                SetStateValues(subscription, decodedEvent);

                await InvokeHandlers(subscription, decodedEvent);
            }
        }

        private async Task InvokeHandlers(IEventSubscription subscription, DecodedEvent decodedEvent)
        {
            foreach (var handler in subscription.EventHandlers)
            {
                if(History != null)
                { 
                    if (await History.ContainsEventHandlerHistory(handler.Id, decodedEvent.Key))
                    {
                        continue;
                    }
                }

                decodedEvent.State["HandlerInvocations"] = 1 + (int)decodedEvent.State["HandlerInvocations"];

                var invokeNextHandler = await handler.HandleAsync(decodedEvent);

                if(History != null)
                { 
                    await History.AddEventHandlerHistory(handler.Id, decodedEvent.Key);
                }

                if (!invokeNextHandler)
                {
                    break;
                }
            }
        }

        private void SetStateValues(IEventSubscription subscription, DecodedEvent decodedEvent)
        {
            subscription.State.Increment("EventsHandled");
            decodedEvent.State["SubscriberId"] = subscription.SubscriberId;
            decodedEvent.State["EventSubscriptionId"] = subscription.Id;
        }

        private bool TryDecode(EventABI[] abis, FilterLog log, out DecodedEvent decodedEvent)
        {
            decodedEvent = null;

            if(abis == null || abis.Length == 0)
            {
                decodedEvent = log.ToDecodedEvent();
                return true;
            }

            var abi = abis.FirstOrDefault(a => log.IsLogForEvent(a.Sha3Signature));
            if (abi is null) return false;

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
