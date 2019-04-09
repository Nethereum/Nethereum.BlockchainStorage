using Nethereum.ABI.Model;
using Nethereum.Contracts.Extensions;
using Nethereum.RPC.Eth.DTOs;

namespace Nethereum.BlockchainProcessing.Processing.Logs.Matching
{
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

        public EventABI[] Abis { get; }
        public IEventAddressMatcher AddressMatcher { get; }
        public IEventParameterMatcher ParameterMatcher { get; }

        private bool MatchesEventSignature(FilterLog log)
        {
            foreach(var abi in Abis)
            {
                if (log.IsLogForEvent(abi.Sha3Signature)) return true;
            }

            return false;
        }

        public bool IsMatch(FilterLog log)
        {
            if (Abis?.Length > 0)
            {
                if (!MatchesEventSignature(log)) return false;
            }

            if (AddressMatcher != null)
            {
                if (!AddressMatcher.IsMatch(log)) return false;
            }

            if (Abis?.Length > 0 && ParameterMatcher != null)
            {
                if(!ParameterMatcher.IsMatch(Abis, log)) return false;
            }

            return true;
        }

    }
}
