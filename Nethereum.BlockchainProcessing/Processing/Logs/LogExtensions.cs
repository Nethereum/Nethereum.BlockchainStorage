using Nethereum.RPC.Eth.DTOs;
using System.Collections.Generic;

namespace Nethereum.BlockchainProcessing.Processing.Logs
{
    public static class LogExtensions
    {
        public static string Key(this FilterLog log)
        {
            if (log.TransactionHash == null || log.LogIndex == null)
                return log.GetHashCode().ToString();

            return $"{log.TransactionHash}{log.LogIndex.HexValue}";
        }

        public static Dictionary<string, FilterLog> Merge(this Dictionary<string, FilterLog> masterList, FilterLog[] candidates)
        {
            foreach (var log in candidates)
            {
                var key = log.Key();

                if (!masterList.ContainsKey(key))
                {
                    masterList.Add(key, log);
                }
            }

            return masterList;
        }

        public static void SetBlockRange(this NewFilterInput filter, BlockRange range)
        {
            filter.FromBlock = new BlockParameter(range.From);
            filter.ToBlock = new BlockParameter(range.To);
        }
    }
}
