using Nethereum.RPC.Eth.DTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Nethereum.BlockchainProcessing.Processing.Logs
{
    public class CatchAllLogProcessor : ILogProcessor
    {
        public CatchAllLogProcessor(Func<IEnumerable<FilterLog>, Task> callBack)
        {
            CallBack = callBack ?? throw new ArgumentNullException(nameof(callBack));
        }

        protected Func<IEnumerable<FilterLog>, Task> CallBack { get; set; }

        public virtual bool IsLogForEvent(FilterLog log) => true;

        public virtual async Task ProcessLogsAsync(params FilterLog[] eventLogs)
        {
            await CallBack(eventLogs).ConfigureAwait(false);
        }
    }
}
