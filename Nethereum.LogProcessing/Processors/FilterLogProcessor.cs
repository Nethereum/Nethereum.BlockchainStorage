using Nethereum.RPC.Eth.DTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Nethereum.BlockchainProcessing.Processing.Logs
{
    public class FilterLogProcessor : ILogProcessor
    {
        public FilterLogProcessor(Predicate<FilterLog> isItForMe, Func<IEnumerable<FilterLog>, Task> callBack)
        {
            IsItForMe = isItForMe ?? throw new ArgumentNullException(nameof(isItForMe));
            CallBack = callBack ?? throw new ArgumentNullException(nameof(callBack));
        }

        public Predicate<FilterLog> IsItForMe { get; }
        protected Func<IEnumerable<FilterLog>, Task> CallBack { get; set; }

        public virtual bool IsLogForEvent(FilterLog log) => IsItForMe(log);

        public virtual async Task ProcessLogsAsync(params FilterLog[] eventLogs)
        {
            await CallBack(eventLogs).ConfigureAwait(false);
        }
    }
}
