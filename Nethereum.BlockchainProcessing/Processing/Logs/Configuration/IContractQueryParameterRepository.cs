using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Nethereum.BlockchainProcessing.Processing.Logs.Configuration
{
    public interface IContractQueryParameterRepository
    {
        Task<IContractQueryParameterDto[]> GetManyAsync(long contractQueryId);
        Task<IContractQueryParameterDto> UpsertAsync(IContractQueryParameterDto dto);
    }
}
