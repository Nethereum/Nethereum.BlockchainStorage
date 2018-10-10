using Newtonsoft.Json.Linq;

namespace Nethereum.BlockchainStore.Handlers
{
    public class TransactionLog
    {
        public TransactionLog(string transactionHash, long logIndex, JObject log)
        {
            TransactionHash = transactionHash;
            LogIndex = logIndex;
            Log = log;
        }

        public string TransactionHash { get; private set; }
        public long LogIndex { get; private set; }
        public JObject Log { get; private set; }

        public string Address => Log == null ? null : (string)Log["address"];
    }
}