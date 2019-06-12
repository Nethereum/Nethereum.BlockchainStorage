using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.BlockchainProcessing.Processing.Logs;
using Nethereum.Contracts.Services;
using Nethereum.RPC.Eth.DTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Nethereum.Contracts
{
    public static class IEthApiContractServiceExtensions
    {
        #region generic (non-event) specific
        public static ILogsProcessorBuilder LogsProcessor
        (this IEthApiContractService ethApiContractService) => new LogsProcessorBuilder(ethApiContractService);

        public static ILogsProcessorBuilder LogsProcessor
        (this IEthApiContractService ethApiContractService, string contractAddress) => new LogsProcessorBuilder(ethApiContractService, contractAddress);

        public static ILogsProcessorBuilder LogsProcessor
        (this IEthApiContractService ethApiContractService, string[] contractAddresses) => new LogsProcessorBuilder(ethApiContractService, contractAddresses);

        public static ILogsProcessorBuilder LogsProcessor
        (this IEthApiContractService ethApiContractService, params NewFilterInput[] filters) => new LogsProcessorBuilder(ethApiContractService, filters);

        # endregion

        #region event specific
        public static ILogsProcessorBuilder<TEventDto> LogsProcessor<TEventDto>
            (this IEthApiContractService ethApiContractService)
            where TEventDto : class, IEventDTO, new() => new LogsProcessorBuilder<TEventDto>(ethApiContractService);


        #region for contract
        public static ILogsProcessorBuilder<TEventDto> LogsProcessor<TEventDto>
            (this IEthApiContractService ethApiContractService, string contractAddress)
        where TEventDto : class, IEventDTO, new()
        {
            return new LogsProcessorBuilder<TEventDto>(ethApiContractService, contractAddress);
        }

        #endregion

        #region for many contracts
        public static ILogsProcessorBuilder<TEventDto> LogsProcessor<TEventDto>
            (this IEthApiContractService ethApiContractService, string[] contractAddresses)
            where TEventDto : class, IEventDTO, new() => new LogsProcessorBuilder<TEventDto>(ethApiContractService, contractAddresses);

        #endregion

        #region with topic specific criteria
        public static ILogsProcessorBuilder<TEventDto> LogsProcessor<TEventDto>
            (this IEthApiContractService ethApiContractService,
            Action<NewFilterInputBuilder<TEventDto>> configureFilterBuilder)
            where TEventDto : class, IEventDTO, new() => new LogsProcessorBuilder<TEventDto>(ethApiContractService, configureFilterBuilder);

        public static ILogsProcessorBuilder<TEventDto> LogsProcessor<TEventDto>
            (this IEthApiContractService ethApiContractService,
            string contractAddress,
            Action<NewFilterInputBuilder<TEventDto>> configureFilterBuilder)
            where TEventDto : class, IEventDTO, new() => new LogsProcessorBuilder<TEventDto>(ethApiContractService, configureFilterBuilder, contractAddress);

        public static ILogsProcessorBuilder<TEventDto> LogsProcessor<TEventDto>
            (this IEthApiContractService ethApiContractService,
            string[] contractAddresses,
            Action<NewFilterInputBuilder<TEventDto>> configureFilterBuilder)
            where TEventDto : class, IEventDTO, new() => new LogsProcessorBuilder<TEventDto>(ethApiContractService, configureFilterBuilder, contractAddresses);

        #endregion

        #endregion

    }
}
