using Microsoft.WindowsAzure.Storage.Table;
using Nethereum.BlockchainProcessing.Processing.Logs.Configuration;
using Nethereum.BlockchainStore.AzureTables.Entities.EventProcessingConfiguration;
using System;
using System.Threading.Tasks;

namespace Nethereum.BlockchainStore.AzureTables.Repositories.EventProcessingConfiguration
{
    public class ContractQueryRepository : AzureTableRepository<ContractQueryEntity>, IContractQueryRepository
    {
        public ContractQueryRepository(CloudTable table) : base(table)
        {
        }

        public async Task<IContractQueryDto> GetContractQueryAsync(long eventHandlerId)
        {
            return await GetAsync(eventHandlerId.ToString(), eventHandlerId.ToString());
        }

        public static ContractQueryEntity Map(IContractQueryDto dto)
        {
            if(dto is ContractQueryEntity e) return e;

            return new ContractQueryEntity
            {
                Id = dto.EventHandlerId,
                ContractAddress = dto.ContractAddress,
                ContractAddressParameterNumber = dto.ContractAddressParameterNumber,
                ContractAddressSource = dto.ContractAddressSource,
                ContractAddressStateVariableName = dto.ContractAddressStateVariableName,
                ContractId = dto.ContractId,
                EventHandlerId = dto.EventHandlerId,
                EventStateOutputName = dto.EventStateOutputName,
                FunctionSignature = dto.FunctionSignature,
                SubscriptionStateOutputName = dto.SubscriptionStateOutputName
            };
        }

        public async Task<IContractQueryDto> UpsertAsync(IContractQueryDto dto)
        {
            var entity = Map(dto);
            return await base.UpsertAsync(entity) as IContractQueryDto;
        }
    }
}
