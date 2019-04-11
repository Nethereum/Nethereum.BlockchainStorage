using Nethereum.ABI.Model;
using Nethereum.Contracts;
using Nethereum.RPC.Eth.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Nethereum.BlockchainProcessing.Processing.Logs.Matching
{
    public class EventParameterMatcher : IEventParameterMatcher
    {
        public EventParameterMatcher(IEnumerable<IParameterCondition> parameterConditions = null)
        {
            ParameterConditions = parameterConditions ?? Array.Empty<IParameterCondition>();
        }

        public IEnumerable<IParameterCondition> ParameterConditions { get; }
        public bool IsMatch(EventABI[] abis, FilterLog log)
        {
            if(!ParameterConditions?.Any() ?? false) return true;

            foreach(var abi in abis)
            {             
                var topics = abi.DecodeEventDefaultTopics(log);
                var matchingFilters = ParameterConditions.Where(f => f.IsTrue(topics)).ToArray();
                if (matchingFilters.Length == ParameterConditions.Count())
                {
                    return true;
                }
            }

            return false;
        }
    }
}
