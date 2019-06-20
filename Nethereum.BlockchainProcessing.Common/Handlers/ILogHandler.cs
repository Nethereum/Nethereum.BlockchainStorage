using Nethereum.RPC.Eth.DTOs;
using System.Threading.Tasks;

namespace Nethereum.BlockchainProcessing.Handlers
{
    public interface ILogHandler
    {
        Task HandleAsync(FilterLog log);
    }
}
