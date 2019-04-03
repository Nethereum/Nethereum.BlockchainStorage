using System;
using System.Collections.Generic;
using System.Text;
using Nethereum.ABI.FunctionEncoding.Attributes;

namespace Nethereum.BlockchainStore.Search
{
    public static class ParameterExtensions
    {
        public static bool IsTuple(this ParameterAttribute parameter)
        {
            return parameter.Type == "tuple";
        }

        public static bool IsTupleArray(this ParameterAttribute parameter)
        {
            return parameter.Type.StartsWith("tuple[");
        }
    }
}
