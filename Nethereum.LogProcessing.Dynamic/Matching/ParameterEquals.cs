using Nethereum.ABI.FunctionEncoding;
using Nethereum.Contracts;
using Nethereum.Hex.HexTypes;
using System;
using System.Collections.Generic;
using System.Numerics;

namespace Nethereum.BlockchainProcessing.Processing.Logs.Matching
{
    /// <summary>
    /// an example of a event parameter filter - simple string equality
    /// </summary>
    public class ParameterEquals : ParameterConditionBase, IParameterCondition
    {
        private readonly bool caseSensitive;

        public ParameterEquals(int parameterOrder, string expectedValue, bool caseSensitive = true)
        {
            ParameterOrder = parameterOrder;
            DecodeTo = typeof(String);
            ExpectedValue = expectedValue;
            this.caseSensitive = caseSensitive;
        }


        private Type DecodeTo {get; set;}
        public string ExpectedValue { get; set; }

        public bool IsTrue(EventLog<List<ParameterOutput>> eventLog)
        {
            var parameterOutput = FindParameter(eventLog);

            if (parameterOutput == null) return false;

            if (parameterOutput.Result == null)
            {
                return ExpectedValue == null;
            }

            object decodedResult = null;

            if(parameterOutput.Result.GetType() == DecodeTo)
            {
                decodedResult = parameterOutput.Result;
            }
            else if(parameterOutput.Result is string r)
            {
                decodedResult = parameterOutput.Parameter.ABIType.Decode(r, DecodeTo); 
            }
            else if(parameterOutput.Result is byte[] b)
            {
                decodedResult = parameterOutput.Parameter.ABIType.Decode(b, DecodeTo); 
            }
            else if(DecodeTo == typeof(string))
            {
                if(parameterOutput.Result is HexBigInteger hbi)
                {
                    decodedResult = hbi.Value.ToString();
                }
                else if(parameterOutput.Result is BigInteger bi)
                {
                    decodedResult = bi.ToString();
                }
                else
                {
                    decodedResult = parameterOutput.Result.ToString();
                }
            }
            else
            {
                throw new Exception($"Result type '{parameterOutput.Result.GetType().Name}' is not supported.");
            }

              
            var decodedAsString = decodedResult is string ? (string)decodedResult : decodedResult.ToString();

            return ExpectedValue.Equals(decodedAsString, caseSensitive ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase);
        }
    }
}
