using System.Numerics;

namespace Nethereum.BlockchainProcessing.Processing.Logs.Matching
{
    public class ParameterLessOrEqual : BigIntegerParameterConditionBase, IParameterCondition
    {
        public ParameterLessOrEqual(int parameterOrder, string val):base(parameterOrder, val){ }

        protected override bool IsTrue(BigInteger value) => value <= Value;
    }
}
