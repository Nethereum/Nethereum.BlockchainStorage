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
        private readonly IEventMatcher _matcher;
        private readonly IEventHandlerCoordinator _handler;

        public EventSubscription(long id, long subscriberId, IEventMatcher matcher, IEventHandlerCoordinator handler)
        {
            Id = id;
            SubscriberId = subscriberId;
            _matcher = matcher ?? throw new System.ArgumentNullException(nameof(matcher));
            _handler = handler ?? throw new System.ArgumentNullException(nameof(handler));
        }

        public long SubscriberId {get; }

        public long Id {get; }

        public bool IsLogForEvent(FilterLog log)
        {
            return _matcher.IsMatch(log);
        }

        public Task ProcessLogsAsync(params FilterLog[] eventLogs)
        {
            return _handler.HandleAsync(_matcher.Abi, eventLogs);
        }
    }
}
