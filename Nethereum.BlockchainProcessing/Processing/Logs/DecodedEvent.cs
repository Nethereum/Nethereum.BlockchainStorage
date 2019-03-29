using Nethereum.ABI.FunctionEncoding;
using Nethereum.Contracts;
using Nethereum.RPC.Eth.DTOs;
using System.Collections.Generic;

namespace Nethereum.BlockchainProcessing.Processing.Logs
{
    public class DecodedEvent
    {
        public Dictionary<string, object> State {get;set;} = new Dictionary<string, object>();
        public EventLog<List<ParameterOutput>> EventLog { get; set; }

        public Transaction Transaction {get;set;}

        public static DecodedEvent Empty()
        {
            return new DecodedEvent{EventLog = new EventLog<List<ParameterOutput>>(new List<ParameterOutput>(), new FilterLog())};
        }
    }
}
