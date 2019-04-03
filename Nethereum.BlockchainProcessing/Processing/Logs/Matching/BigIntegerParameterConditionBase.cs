using Nethereum.ABI.FunctionEncoding;
using Nethereum.Contracts;
using System.Collections.Generic;
using System.Numerics;

namespace Nethereum.BlockchainProcessing.Processing.Logs.Matching
{
    public abstract class BigIntegerParameterConditionBase : ParameterConditionBase
    {
        protected BigIntegerParameterConditionBase(int parameterOrder, string val)
        {
            ParameterOrder = parameterOrder;
            Value = BigInteger.Parse(val);
        }

        public BigInteger Value { get; }

        protected abstract bool IsTrue(BigInteger value);

        public bool IsTrue(EventLog<List<ParameterOutput>> eventLog)
        {
            var parameterOutput = FindParameter(eventLog);

            if (parameterOutput == null) return false;

            if(parameterOutput.Result is BigInteger bi)
            {
                return IsTrue(bi);
            }
            
            return false;
        }
    }
}
