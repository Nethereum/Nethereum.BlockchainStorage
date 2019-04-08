using Nethereum.BlockchainProcessing.Processing.Logs.Handling;
using Nethereum.BlockchainProcessing.Processing.Logs.Matching;
using Nethereum.RPC.Eth.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Nethereum.BlockchainProcessing.Processing.Logs
{
    /// <summary>
    /// a dynamically loaded subscription
    /// Designed to be instantiated from DB configuration data
    /// </summary>

    public class EventSubscription : IEventSubscription
    {
        public EventSubscription(
            long id, 
            long subscriberId, 
            IEventMatcher matcher, 
            IEventHandlerManager handlerManager, 
            EventSubscriptionStateDto state)
        {
            Id = id;
            SubscriberId = subscriberId;
            Matcher = matcher ?? throw new System.ArgumentNullException(nameof(matcher));
            HandlerManager = handlerManager ?? throw new System.ArgumentNullException(nameof(handlerManager));
            State = state;
            Handlers = new List<IEventHandler>();

            if (!State.Values.ContainsKey("HandlerInvocations"))
            {
                State.SetInt("HandlerInvocations", 0);
            }
        }

        public void AddHandler(IEventHandler handler)
        {
            Handlers.Add(handler);
        }

        public IEnumerable<IEventHandler> EventHandlers => Handlers.AsReadOnly();

        private List<IEventHandler> Handlers { get; }

        public long SubscriberId {get; }

        public long Id {get; }

        public IEventHandlerManager HandlerManager { get; }
        public EventSubscriptionStateDto State { get; }
        public IEventMatcher Matcher { get; }

        public bool IsLogForEvent(FilterLog log)
        {
            return Matcher.IsMatch(log);
        }

        public Task ProcessLogsAsync(params FilterLog[] eventLogs)
        {
            State.Increment("HandlerInvocations");
            return HandlerManager.HandleAsync(this, Matcher.Abi, eventLogs);
        }
    }
}
