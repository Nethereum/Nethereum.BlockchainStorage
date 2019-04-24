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
            ISubscriberContractsRepository subscriberContractsRepository,
            IContractQueryParameterRepository contractQueryParameterRepository)
        {
            var queryConfig = await contractQueryRepository.GetContractQueryAsync(eventHandlerId);
            var contractConfig = await subscriberContractsRepository.GetContractAsync(subscriberId, queryConfig.ContractId);
            var queryParameters = await contractQueryParameterRepository.GetAsync(queryConfig.Id);

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

        public static EventAggregatorConfiguration ToEventAggregatorConfiguration(this IEventAggregatorDto dto)
        {
            return new EventAggregatorConfiguration
            {
                Destination = dto.Destination,
                EventParameterNumber = dto.EventParameterNumber,
                SourceKey = dto.SourceKey,
                Operation = dto.Operation,
                OutputKey = dto.OutputKey,
                Source = dto.Source
            };
        }
    }
}
