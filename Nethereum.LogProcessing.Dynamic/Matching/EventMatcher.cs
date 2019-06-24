using Nethereum.ABI.Model;
using Nethereum.Contracts;
using Nethereum.RPC.Eth.DTOs;
using System;
using System.Linq;

namespace Nethereum.BlockchainProcessing.Processing.Logs.Matching
{
    public class EventMatcher<TEvent>: EventMatcher
    {
        public EventMatcher(
            IEventAddressMatcher addressMatcher = null,
            IEventParameterMatcher parameterMatcher = null)
            :base(null, addressMatcher, parameterMatcher)
        {
            Abis = new[]{ABITypedRegistry.GetEvent<TEvent>() };
        }
    }

    /// <summary>
    /// encapsulate IsLogForEvent implementation
    /// event matcher
    /// wraps up event signature checks, address checks and parameter value matching
    /// </summary>
    public class EventMatcher : IEventMatcher
    {
        public EventMatcher(
            EventABI[] eventAbis = null,
            IEventAddressMatcher addressMatcher = null,
            IEventParameterMatcher parameterMatcher = null)
        {
            Abis = eventAbis;
            AddressMatcher = addressMatcher;
            ParameterMatcher = parameterMatcher;
        }

        protected EventMatcher() { }

        public EventABI[] Abis { get; protected set; }
        public IEventAddressMatcher AddressMatcher { get; }
        public IEventParameterMatcher ParameterMatcher { get; }

        protected virtual bool MatchesEventSignature(FilterLog log)
        {
            if (Abis == null || Abis.Length == 0)
            {
                return true;
            }

            foreach (var abi in Abis)
            {
                if (log.IsLogForEvent(abi.Sha3Signature)) return true;
            }

            return false;
        }

        protected virtual bool MatchesParameter(FilterLog log)
        {
            if (Abis?.Length > 0 && ParameterMatcher != null)
            {
                if (!ParameterMatcher.IsMatch(Abis, log)) return false;
            }
            return true;
        }

        public bool IsMatch(FilterLog log)
        {
            if (!MatchesEventSignature(log)) return false;

            if (AddressMatcher != null)
            {
                if (!AddressMatcher.IsMatch(log)) return false;
            }

            if(!MatchesParameter(log)) return false;

            return true;
        }

    }
}
