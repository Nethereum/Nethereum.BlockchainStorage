using Nethereum.ABI.Model;
using Nethereum.Contracts.Extensions;
using Nethereum.RPC.Eth.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Nethereum.BlockchainProcessing.Processing.Logs.Matching
{
    public class EventParameterMatcher : IEventParameterMatcher
    {
        public EventParameterMatcher(IEnumerable<IParameterCondition> parameterConditions)
        {
            ParameterConditions = parameterConditions
                ?? throw new ArgumentNullException(nameof(parameterConditions));
        }

        public IEnumerable<IParameterCondition> ParameterConditions { get; }
        public bool IsMatch(EventABI abi, FilterLog log)
        {
            if(!ParameterConditions?.Any() ?? false) return true;

            var topics = abi.DecodeEventDefaultTopics(log);
            var matchingFilters = ParameterConditions.Where(f => f.IsTrue(topics)).ToArray();
            if (matchingFilters.Length != ParameterConditions.Count())
            {
                return false;
            }
            return true;
        }
    }
}
