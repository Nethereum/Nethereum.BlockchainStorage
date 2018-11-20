using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Nethereum.RPC.Eth.DTOs;

namespace Nethereum.BlockchainProcessing.Processing.Logs
{
    public interface ILogProcessor
    {
        bool IsLogForEvent(FilterLog log);
        Task ProcessLogsAsync(params FilterLog[] eventLogs);
    }
}
