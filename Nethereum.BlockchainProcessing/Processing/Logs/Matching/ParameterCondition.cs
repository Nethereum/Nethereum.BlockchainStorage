using System;

namespace Nethereum.BlockchainProcessing.Processing.Logs.Matching
{
    public static class ParameterCondition
    {
        public static IParameterCondition Create(int parameterOrder, string _operator, string expectedValue)
        {
            if(_operator == "=")
            {
                return Equals(parameterOrder, expectedValue);
            }
            throw new ArgumentException(nameof(_operator));
        }

        public static IParameterCondition Equals(int parameterOrder, string expectedValue)
        {
             return new ParameterEquals(parameterOrder, expectedValue);
        } 
    }
}
