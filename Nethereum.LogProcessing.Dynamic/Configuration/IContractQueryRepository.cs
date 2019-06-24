using System.Threading.Tasks;

namespace Nethereum.BlockchainProcessing.Processing.Logs.Configuration
{
    public interface IContractQueryRepository
    {
        Task<IContractQueryDto> GetAsync(long eventHandlerId);
        Task<IContractQueryDto> UpsertAsync(IContractQueryDto dto);
    }


}
