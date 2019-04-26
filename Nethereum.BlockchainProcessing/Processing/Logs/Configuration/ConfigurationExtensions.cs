using Nethereum.BlockchainProcessing.Processing.Logs.Handling.Handlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nethereum.BlockchainProcessing.Processing.Logs.Configuration
{
    public static class ConfigurationExtensions
    {
        public static async Task<ContractQueryConfiguration> LoadContractQueryConfiguration(
            this IContractQueryRepository contractQueryRepository,
            long subscriberId,
            long eventHandlerId,
            ISubscriberOwnedRepository<ISubscriberContractDto> subscriberContractsRepository,
            IContractQueryParameterRepository contractQueryParameterRepository)
        {
            var queryConfig = await contractQueryRepository.GetAsync(eventHandlerId);
            var contractConfig = await subscriberContractsRepository.GetAsync(subscriberId, queryConfig.ContractId);
            var queryParameters = await contractQueryParameterRepository.GetManyAsync(queryConfig.Id);

            return queryConfig.ToContractQueryConfiguration(contractConfig, queryParameters);
        }

        public static ContractQueryConfiguration ToContractQueryConfiguration(
            this IContractQueryDto queryConfig, 
            ISubscriberContractDto contractConfig, 
            IContractQueryParameterDto[] queryParameters)
        {
            return new ContractQueryConfiguration
            {
                ContractAddress = queryConfig.ContractAddress,
                ContractAddressParameterNumber = queryConfig.ContractAddressParameterNumber,
                ContractAddressSource = queryConfig.ContractAddressSource,
                ContractAddressStateVariableName = queryConfig.ContractAddressStateVariableName,
                EventStateOutputName = queryConfig.EventStateOutputName,
                FunctionSignature = queryConfig.FunctionSignature,
                SubscriptionStateOutputName = queryConfig.SubscriptionStateOutputName,
                ContractABI = contractConfig.Abi,
                Parameters = queryParameters.Select(p => new ContractQueryParameter
                {
                    EventParameterNumber = p.EventParameterNumber,
                    EventStateName = p.EventStateName,
                    Order = p.Order,
                    Source = p.Source,
                    Value = p.Value
                }).ToArray()
            };
        }
    }
}
