using Microsoft.WindowsAzure.Storage.Table;
using Nethereum.BlockchainProcessing.Processing.Logs.Configuration;
using Nethereum.BlockchainStore.AzureTables.Entities.EventProcessingConfiguration;
using System.Threading.Tasks;

namespace Nethereum.BlockchainStore.AzureTables.Repositories.EventProcessingConfiguration
{
    public class ContractQueryParameterRepository : AzureTableRepository<ContractQueryParameterEntity>, IContractQueryParameterRepository
    {
        public ContractQueryParameterRepository(CloudTable table) : base(table)
        {
        }

        public async Task<IContractQueryParameterDto[]> GetAsync(long contractQueryId)
        {
            return await GetManyAsync(contractQueryId.ToString());
        }

        public ContractQueryParameterEntity Map(IContractQueryParameterDto dto)
        {
            if(dto is ContractQueryParameterEntity e) return e;

            return new ContractQueryParameterEntity
            {
                ContractQueryId = dto.ContractQueryId,
                EventParameterNumber = dto.EventParameterNumber,
                EventStateName = dto.EventStateName,
                Id = dto.Id,
                Order = dto.Order,
                Source = dto.Source,
                Value = dto.Value
            };
        }

        public async Task<IContractQueryParameterDto> UpsertAsync(IContractQueryParameterDto dto)
        {
            var entity = Map(dto);
            return await base.UpsertAsync(entity) as IContractQueryParameterDto;
        }
    }
}
