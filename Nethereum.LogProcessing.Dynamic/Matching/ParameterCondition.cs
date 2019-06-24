using Nethereum.BlockchainProcessing.Processing.Logs.Configuration;
using System;

namespace Nethereum.BlockchainProcessing.Processing.Logs.Matching
{
    public static class ParameterCondition
    {
        public static IParameterCondition Create(int parameterOrder, ParameterConditionOperator @operator, string value)
        {
            if(@operator == ParameterConditionOperator.Equals)
            {
                return new ParameterEquals(parameterOrder, value);
            }
            else if(@operator == ParameterConditionOperator.GreaterOrEqual)
            {
                return new ParameterGreaterOrEqual(parameterOrder, value);
            }
            else if(@operator == ParameterConditionOperator.LessOrEqual)
            {
                return new ParameterLessOrEqual(parameterOrder, value);
            }
            throw new ArgumentException(nameof(@operator));
        }

    }
}
