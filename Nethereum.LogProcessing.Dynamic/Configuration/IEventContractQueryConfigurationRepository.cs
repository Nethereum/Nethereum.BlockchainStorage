using Nethereum.BlockchainProcessing.Processing.Logs.Handling.Handlers;
using System.Threading.Tasks;

namespace Nethereum.BlockchainProcessing.Processing.Logs.Configuration
{
    public interface IEventContractQueryConfigurationRepository
    {
        Task<ContractQueryConfiguration> GetAsync(long subscriberId, long eventHandlerId);
    }
}
