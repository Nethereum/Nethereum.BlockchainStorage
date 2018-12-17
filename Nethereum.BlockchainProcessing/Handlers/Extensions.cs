using Nethereum.ABI;
using Nethereum.Hex.HexTypes;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace Nethereum.BlockchainProcessing.Handlers
{
    public static class Extensions
    {
        private static readonly JArray EmptyJArray = new JArray();
        private static readonly AddressType AddressType = new AddressType();

        public static IEnumerable<string> GetContractsCalled(this TransactionVmStack vmStack)
        {
            return vmStack
                .GetStructLogs()
                .GetCallOpCodes()
                .Select(c => c.Address)
                .Distinct(StringComparer.InvariantCultureIgnoreCase);
        }

        public static JArray GetStructLogs(this TransactionVmStack vmStack)
        {
            if (vmStack?.StackTrace == null) return EmptyJArray;

            if (vmStack.StackTrace["structLogs"] is JArray structLogs)
            {
                return structLogs;
            }

            return EmptyJArray;
        }

        public static IEnumerable<CallOpCode> GetCallOpCodes(this JArray structLogs)
        {
            return structLogs
                .Where(l => l["op"]?.Value<string>() == "CALL")
                .Select(l => l.ToCallOpCode());
        }

        public static CallOpCode ToCallOpCode(this JToken token)
        {
            var stackArray = token["stack"] as JArray;

            if(stackArray == null) return CallOpCode.Empty;

            return new CallOpCode
            {
                Op = token["op"]?.Value<string>(),
                Gas = stackArray[stackArray.Count - 1].ToBigInteger(),
                Address = stackArray[stackArray.Count - 2].ConvertStructLogValueToAddress(),
                EtherValue = stackArray[stackArray.Count - 3].ToBigInteger()
            };
        }

        public static HexBigInteger ToBigInteger(this JToken token)
        {
            return new HexBigInteger(token.Value<string>());
        }

        public static string ConvertStructLogValueToAddress(this JToken token)
        {
            var rawValue = token.Value<string>();
            return AddressType.Decode(rawValue, typeof(string)) as string;
        }
        
    }

    public struct CallOpCode
    {
        public string Op { get; set; }
        public string Address { get; set; }
        public BigInteger EtherValue { get; set; }
        public BigInteger Gas { get; set; }

        public static readonly CallOpCode Empty = new CallOpCode();
    }
}
