using System.Threading.Tasks;
using Nethereum.RPC.Eth.DTOs;

namespace Nethereum.BlockchainProcessing.BlockchainProxy
{
    public interface IEventLogProxy
    {
        Task<FilterLog[]> GetLogs(NewFilterInput newFilter, object id = null);
    }
}
