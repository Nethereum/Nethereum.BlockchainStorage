using System.Threading.Tasks;

namespace Nethereum.BlockchainProcessing.Processing.Logs.Handling
{
    public interface IEventContractQueryFactory
    {
        Task<ContractQueryConfiguration> GetContractQueryAsync(long eventSubscriptionId);
    }
}
