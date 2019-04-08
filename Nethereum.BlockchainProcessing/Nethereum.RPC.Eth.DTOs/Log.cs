using System.Linq;
using Nethereum.ABI.FunctionEncoding;
using Nethereum.Contracts;

namespace Nethereum.RPC.Eth.DTOs
{
    public static class LogExtensions
    {
        public static string EventSignature(this FilterLog log) => log.GetTopic(0);
        public static string IndexedVal1(this FilterLog log) => log.GetTopic(1);
        public static string IndexedVal2(this FilterLog log) => log.GetTopic(2);
        public static string IndexedVal3(this FilterLog log) => log.GetTopic(3);
        private static string GetTopic(this FilterLog log, int number)
        {
            if (log.Topics == null) return null;

            if (log.Topics.Length > number)
                return log.Topics[number].ToString();

            return null;
        }

    }

}
