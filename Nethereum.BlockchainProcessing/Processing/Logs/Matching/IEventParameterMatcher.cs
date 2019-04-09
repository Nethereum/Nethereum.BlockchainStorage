using Nethereum.ABI.Model;
using Nethereum.RPC.Eth.DTOs;

namespace Nethereum.BlockchainProcessing.Processing.Logs.Matching
{
    public interface IEventParameterMatcher 
    {
        bool IsMatch(EventABI[] abis, FilterLog log);
    }
}
