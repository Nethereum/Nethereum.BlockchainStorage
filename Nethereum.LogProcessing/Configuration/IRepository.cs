using System.Collections.Generic;
using System.Threading.Tasks;

namespace Nethereum.BlockchainProcessing.Processing.Logs.Configuration
{
    public interface IRepository<TDto>
    {
        Task<TDto> UpsertAsync(TDto dto);
        Task UpsertAsync(IEnumerable<TDto> dtos);
    }
}
