using Nethereum.Contracts;
using Nethereum.RPC.Eth.DTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Nethereum.BlockchainProcessing.Processing.Logs
{
    public class LogProcessor<TEventDto> : LogProcessorBase<TEventDto> where TEventDto : class, new()
    {
        public LogProcessor(Func<IEnumerable<EventLog<TEventDto>>, Task> callBack)
        {
            CallBack = callBack;
        }

        protected Func<IEnumerable<EventLog<TEventDto>>, Task> CallBack { get; set; }

        public override async Task ProcessLogsAsync(params FilterLog[] eventLogs)
        {
            var list = eventLogs.DecodeAllEventsIgnoringIndexMisMatches<TEventDto>();
            await CallBack(list);
        }
    }
}
