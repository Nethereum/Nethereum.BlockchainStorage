using Nethereum.RPC.Eth.DTOs;
using System.Threading.Tasks;

namespace Nethereum.BlockchainProcessing.Web3Abstractions
{
    public interface IEventLogProxy
    {
        Task<FilterLog[]> GetLogs(NewFilterInput newFilter, object id = null);
    }
}
