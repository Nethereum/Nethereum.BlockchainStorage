using Nethereum.ABI.Model;
using Nethereum.RPC.Eth.DTOs;

namespace Nethereum.BlockchainProcessing.Processing.Logs.Matching
{
    public interface IEventMatcher
    {
        EventABI[] Abis {get;}
        bool IsMatch(FilterLog log);
    }
}