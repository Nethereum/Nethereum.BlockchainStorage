using Nethereum.BlockchainProcessing.Processing.Logs.Handling.Handlers;
using System.Threading.Tasks;

namespace Nethereum.BlockchainProcessing.Processing.Logs.Configuration
{
    public class EventContractQueryConfigurationRepository: IEventContractQueryConfigurationRepository
    {
        public EventContractQueryConfigurationRepository(
            IContractQueryRepository queryRepo,
            ISubscriberOwnedRepository<ISubscriberContractDto> contractRepo, 
            IContractQueryParameterRepository parameterRepo)
        {
            QueryRepo = queryRepo;
            ContractRepo = contractRepo;
            ParameterRepo = parameterRepo;
        }

        public IContractQueryRepository QueryRepo { get; }
        public ISubscriberOwnedRepository<ISubscriberContractDto> ContractRepo { get; }
        public IContractQueryParameterRepository ParameterRepo { get; }

        public async Task<ContractQueryConfiguration> GetContractQueryConfigurationAsync(long subscriberId, long eventHandlerId)
        {
            return await QueryRepo.LoadContractQueryConfiguration(
                subscriberId, eventHandlerId, ContractRepo, ParameterRepo);
        }
    }
}
