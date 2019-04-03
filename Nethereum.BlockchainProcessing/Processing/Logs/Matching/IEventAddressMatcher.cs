using Nethereum.RPC.Eth.DTOs;

namespace Nethereum.BlockchainProcessing.Processing.Logs.Matching
{
    public interface IEventAddressMatcher
    {
        bool IsMatch(FilterLog log);
    }
}
