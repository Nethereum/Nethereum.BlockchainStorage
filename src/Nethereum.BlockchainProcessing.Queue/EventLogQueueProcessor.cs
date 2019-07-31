using Nethereum.BlockchainProcessing.Processor;
using Nethereum.RPC.Eth.DTOs;
using System;
using System.Threading.Tasks;

namespace Nethereum.BlockchainProcessing.Queue
{
    public class EventLogQueueProcessor : ProcessorHandler<FilterLog>
    {
        public EventLogQueueProcessor(
            IQueue destinationQueue, 
            Func<FilterLog, object> mapper = null) :
                base((filterLog) => destinationQueue.AddMessageAsync(mapper?.Invoke(filterLog) ?? filterLog))
        { }

        public EventLogQueueProcessor(
            IQueue destinationQueue, 
            Func<FilterLog, Task<bool>> criteria,
            Func<FilterLog, object> mapper = null) :
                base((filterLog) => destinationQueue.AddMessageAsync(mapper?.Invoke(filterLog) ?? filterLog)) 
        { }

        public EventLogQueueProcessor(
            IQueue destinationQueue, 
            Func<FilterLog, bool> criteria,
            Func<FilterLog, object> mapper = null) : 
                base((filterLog) => destinationQueue.AddMessageAsync(mapper?.Invoke(filterLog) ?? filterLog), criteria) 
        { }

    }
}
