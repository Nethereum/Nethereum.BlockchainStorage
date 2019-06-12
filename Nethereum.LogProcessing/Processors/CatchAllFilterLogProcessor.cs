using Nethereum.RPC.Eth.DTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Nethereum.BlockchainProcessing.Processing.Logs
{
    public class CatchAllFilterLogProcessor : FilterLogProcessor
    {
        public CatchAllFilterLogProcessor(Func<IEnumerable<FilterLog>, Task> callBack):base((log) => true, callBack)
        {
            CallBack = callBack ?? throw new ArgumentNullException(nameof(callBack));
        }
    }
}
