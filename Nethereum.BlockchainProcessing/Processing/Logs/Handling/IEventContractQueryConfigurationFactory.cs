using System.Threading.Tasks;

namespace Nethereum.BlockchainProcessing.Processing.Logs.Handling
{
    public interface IEventContractQueryConfigurationFactory
    {
        Task<ContractQueryConfiguration> GetContractQueryConfigurationAsync(long eventSubscriptionId);
    }
}
