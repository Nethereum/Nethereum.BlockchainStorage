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
            EventABI eventAbi = null,
            IEventAddressMatcher addressMatcher = null,
            IEventParameterMatcher parameterMatcher = null)
        {
            Abi = eventAbi;
            AddressMatcher = addressMatcher;
            ParameterMatcher = parameterMatcher;
        }

        public EventABI Abi { get; }
        public IEventAddressMatcher AddressMatcher { get; }
        public IEventParameterMatcher ParameterMatcher { get; }

        public bool IsMatch(FilterLog log)
        {
            if (Abi != null)
            {
                if (!log.IsLogForEvent(Abi.Sha3Signature)) return false;
            }

            if (AddressMatcher != null)
            {
                if (!AddressMatcher.IsMatch(log)) return false;
            }

            if (Abi != null && ParameterMatcher != null)
            {
                if(!ParameterMatcher.IsMatch(Abi, log)) return false;
            }

            return true;
        }

    }
}
