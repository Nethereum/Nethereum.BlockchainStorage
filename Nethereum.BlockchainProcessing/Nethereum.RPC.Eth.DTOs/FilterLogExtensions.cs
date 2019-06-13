using Nethereum.BlockchainProcessing.Processing;
using Nethereum.Contracts;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Nethereum.RPC.Eth.DTOs
{
    public static class FilterLogExtensions
    {
        // an internal cache of index mismatch decoding signatures
        // used to intercept attempts to decode logs with the same signature and throwing unecessary errors 
        private static readonly ConcurrentDictionary<Type, ConcurrentDictionary<int, bool>> _invalidTopicLengthsPerEvent = 
            new ConcurrentDictionary<Type, ConcurrentDictionary<int, bool>>();

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
            var eventDtoType = typeof(TEventDto);
            eventLog = null;
            try
            {
                if(log.MatchesPreviousDecodingIndexMismatch(eventDtoType)) return false;

                eventLog = log.DecodeEvent<TEventDto>();

                return true;
            }
            catch (Exception ex) when (ex.IsEventDecodingIndexMisMatch())
            {
                CacheDecodingIndexMismatch(log, eventDtoType);

                //ignore;
                return false;
            }
        }

        private static bool MatchesPreviousDecodingIndexMismatch(this FilterLog log, Type eventDtoType)
        {
            if(!_invalidTopicLengthsPerEvent.Any()) return false;

            if(_invalidTopicLengthsPerEvent.TryGetValue(eventDtoType, out ConcurrentDictionary<int, bool> topicLengths))
            {
                if(topicLengths.TryGetValue(log.Topics.Length, out _)) return true;
            }

            return false;

        }

        private static void CacheDecodingIndexMismatch(FilterLog log, Type eventDtoType)
        {
            if (!_invalidTopicLengthsPerEvent.ContainsKey(eventDtoType))
            {
                _invalidTopicLengthsPerEvent.TryAdd(eventDtoType, new ConcurrentDictionary<int, bool>());
            }

            if (!_invalidTopicLengthsPerEvent.TryGetValue(eventDtoType, out ConcurrentDictionary<int, bool> topicLengths))
            {
                if (!topicLengths.ContainsKey(log.Topics.Length))
                {
                    topicLengths.TryAdd(log.Topics.Length, true);
                }
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

        public static FilterLog[] Sort(this IEnumerable<FilterLog> logs)
        {
            return logs
                .OrderBy(l => l.BlockNumber?.Value)
                .ThenBy(l => l.TransactionIndex?.Value)
                .ThenBy(l => l.LogIndex?.Value)
                .ToArray();
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
