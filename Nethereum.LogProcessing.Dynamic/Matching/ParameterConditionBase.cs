using Nethereum.ABI.FunctionEncoding;
using Nethereum.Contracts;
using System.Collections.Generic;
using System.Linq;

namespace Nethereum.BlockchainProcessing.Processing.Logs.Matching
{
    public class ParameterConditionBase
    {
        public int ParameterOrder {get; set;}

        protected ParameterOutput FindParameter(EventLog<List<ParameterOutput>> eventLog)
        {
            return eventLog.Event.FirstOrDefault(p => p.Parameter.Order == ParameterOrder);
        }
    }
}
