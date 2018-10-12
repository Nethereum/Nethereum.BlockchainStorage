using System.Linq;
using Nethereum.ABI.FunctionEncoding;
using Nethereum.Contracts;

namespace Nethereum.RPC.Eth.DTOs
{
    public class Log: FilterLog
    {
        public string EventSignature => GetIndexedTopic(0);
        public string IndexedVal1 => GetIndexedTopic(1);
        public string IndexedVal2 => GetIndexedTopic(2);
        public string IndexedVal3 => GetIndexedTopic(3);

        private string GetIndexedTopic(int number)
        {
            if (Topics == null) return null;

            if (Topics.Length > number)
                return Topics[number].ToString();

            return null;
        }

        public T DecodeEvent<T>(Event eventHandler) where T: new()
        {
            if (!eventHandler.IsLogForEvent(this)) return default(T);

            var decoder = new EventTopicDecoder();
            return decoder.DecodeTopics<T>(Topics, Data);
        }
    }
}
