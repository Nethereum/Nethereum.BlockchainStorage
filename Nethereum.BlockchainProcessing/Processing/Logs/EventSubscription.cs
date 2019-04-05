using Nethereum.BlockchainProcessing.Processing.Logs.Handling;
using Nethereum.BlockchainProcessing.Processing.Logs.Matching;
using Nethereum.RPC.Eth.DTOs;
using System.Threading.Tasks;

namespace Nethereum.BlockchainProcessing.Processing.Logs
{
    /// <summary>
    /// a dynamically loaded subscription
    /// Designed to be instantiated from DB configuration data
    /// </summary>

    public class EventSubscription : IEventSubscription
    {
        public EventSubscription(long id, long subscriberId, IEventMatcher matcher, IEventHandlerCoordinator handler, EventSubscriptionStateDto state)
        {
            Id = id;
            SubscriberId = subscriberId;
            Matcher = matcher ?? throw new System.ArgumentNullException(nameof(matcher));
            Handler = handler ?? throw new System.ArgumentNullException(nameof(handler));
            State = state;
        }

        public long SubscriberId {get; }

        public long Id {get; }

        public IEventHandlerCoordinator Handler { get; }
        public EventSubscriptionStateDto State { get; }
        public IEventMatcher Matcher { get; }

        public bool IsLogForEvent(FilterLog log)
        {
            return Matcher.IsMatch(log);
        }

        public Task ProcessLogsAsync(params FilterLog[] eventLogs)
        {
            return Handler.HandleAsync(Matcher.Abi, eventLogs);
        }
    }
}
