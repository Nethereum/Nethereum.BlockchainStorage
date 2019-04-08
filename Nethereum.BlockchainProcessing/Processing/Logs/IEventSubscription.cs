using Nethereum.BlockchainProcessing.Processing.Logs.Handling;
using System.Collections;
using System.Collections.Generic;

namespace Nethereum.BlockchainProcessing.Processing.Logs
{
    public interface IEventSubscription: ILogProcessor
    {
        long Id {get;}
        long SubscriberId {get;}

        EventSubscriptionStateDto State { get; }

        IEnumerable<IEventHandler> EventHandlers { get;}
    }
}
