using System;

namespace Nethereum.BlockchainProcessing.Processing.Logs.Matching
{
    public static class ParameterCondition
    {
        public static IParameterCondition Create(int parameterOrder, string _operator, string value)
        {
            if(_operator == "=")
            {
                return new ParameterEquals(parameterOrder, value);
            }
            else if(_operator == "=>")
            {
                return new ParameterGreaterOrEqual(parameterOrder, value);
            }
            else if(_operator == "<=")
            {
                return new ParameterLessOrEqual(parameterOrder, value);
            }
            throw new ArgumentException(nameof(_operator));
        }

    }
}
