using Nethereum.ABI.FunctionEncoding;
using Nethereum.Contracts;
using Nethereum.RPC.Eth.DTOs;
using System.Collections.Generic;

namespace Nethereum.BlockchainProcessing.Processing.Logs
{
    public class DecodedEvent
    {
        public Dictionary<string, object> Aggregates {get;set;} = new Dictionary<string, object>();
        public Dictionary<string, object> Metadata {get;set;} = new Dictionary<string, object>();
        public EventLog<List<ParameterOutput>> Log { get; set; }

        public static DecodedEvent Empty()
        {
            return new DecodedEvent{Log = new EventLog<List<ParameterOutput>>(new List<ParameterOutput>(), new FilterLog())};
        }
    }
}
