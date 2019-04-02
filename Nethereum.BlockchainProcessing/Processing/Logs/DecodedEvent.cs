using Nethereum.ABI.FunctionEncoding;
using Nethereum.Contracts;
using Nethereum.RPC.Eth.DTOs;
using System.Collections.Generic;

namespace Nethereum.BlockchainProcessing.Processing.Logs
{
    public class DecodedEvent: EventLog<List<ParameterOutput>>
    {
        public DecodedEvent(List<ParameterOutput> outputs, FilterLog log):base(outputs, log)
        {

        }

        public Dictionary<string, object> State {get;set;} = new Dictionary<string, object>();

        public Transaction Transaction {get;set;}

        public static DecodedEvent Empty()
        {
            return new DecodedEvent(new List<ParameterOutput>(), new FilterLog());
        }
    }
}
