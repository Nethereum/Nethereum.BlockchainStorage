using System;
using Nethereum.RPC.Eth.DTOs;
using System.Collections.Generic;
using System.Linq;
using Nethereum.Contracts;

namespace Nethereum.BlockchainProcessing.Processing.Logs
{
    public static class LogExtensions
    {
        public static List<EventLog<TEventDto>> DecodeAllEventsIgnoringIndexMisMatches<TEventDto>(this FilterLog[] logs) where TEventDto : class, new()
        {
            var list = new List<EventLog<TEventDto>>(logs.Length);

            foreach (var log in logs)
            {
                if (log.TryDecodeEvent(out EventLog<TEventDto> eventLog))
                {
                    list.Add(eventLog);
                }
            }

            return list;
        }


        public static bool TryDecodeEvent<TEventDto>(this FilterLog log, out EventLog<TEventDto> eventLog) where TEventDto : class, new()
        {
            eventLog = null;
            try
            {
                eventLog = log.DecodeEvent<TEventDto>();
                return true;
            }
            catch (Exception ex) when (ex.IsEventDecodingIndexMisMatch())
            {
                //ignore;
                return false;
            }
        }

        public static bool IsEventDecodingIndexMisMatch(this Exception ex)
        {
            return ex.Message.StartsWith("Number of indexes don't match the number of topics");
        }
        
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

        public static bool IsTopicFiltered(this NewFilterInput filter, uint topicNumber)
        {
            var filterValue = filter.GetFirstTopicValue(topicNumber);
            return filterValue != null;
        }

        public static string GetFirstTopicValueAsString(this NewFilterInput filter, uint topicNumber)
        {
            var filterValue = filter.GetFirstTopicValue(topicNumber);
            return filterValue?.ToString();
        }

        public static object GetFirstTopicValue(this NewFilterInput filter, uint topicNumber)
        {
            var topicValues = filter.GetTopicValues(topicNumber);
            return topicValues.FirstOrDefault();
        }

        public static object[] GetTopicValues(this NewFilterInput filter, uint topicNumber)
        {
            var allTopics = filter.Topics;

            if (allTopics == null) return Array.Empty<object>();
            if (allTopics.Length < 2) return Array.Empty<object>();
            if (topicNumber > allTopics.Length) return Array.Empty<object>();

            if (allTopics[topicNumber] is object[] topicValues)
                return topicValues;

            if (allTopics[topicNumber] is object val)
                return new [] {val};

            return Array.Empty<object>();
        }
    }
}
