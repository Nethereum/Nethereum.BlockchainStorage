using Microsoft.WindowsAzure.Storage.Table;
using Nethereum.BlockchainProcessing.Processing.Logs.Configuration;
using Nethereum.BlockchainStore.AzureTables.Entities.EventProcessingConfiguration;

namespace Nethereum.BlockchainStore.AzureTables.Repositories.EventProcessingConfiguration
{
    public class ContractQueryParameterRepository : 
        OwnedRepository<IContractQueryParameterDto, ContractQueryParameterEntity>, 
        IContractQueryParameterRepository
    {
        public ContractQueryParameterRepository(CloudTable table) : base(table)
        {
        }

        protected override ContractQueryParameterEntity Map(IContractQueryParameterDto dto)
        {
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
    }
}
