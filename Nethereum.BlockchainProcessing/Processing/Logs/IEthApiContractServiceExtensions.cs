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
        public static LogsProcessorBuilder LogsProcessor
        (this IEthApiContractService ethApiContractService)
        {
            return new LogsProcessorBuilder(ethApiContractService);
        }

        public static LogsProcessorBuilder LogsProcessor
        (this IEthApiContractService ethApiContractService, params NewFilterInput[] filters)
        {
            return new LogsProcessorBuilder(ethApiContractService, filters);
        }

        public static LogsProcessorBuilder LogsProcessor
        (this IEthApiContractService ethApiContractService, NewFilterInput filter)
        {
            return new LogsProcessorBuilder(ethApiContractService, filter);
        }

        public static LogsProcessorBuilder LogsProcessor
        (this IEthApiContractService ethApiContractService,
        string contractAddress)
        {
            return new LogsProcessorBuilder(ethApiContractService, contractAddress);
        }

        public static LogsProcessorBuilder LogsProcessor
        (this IEthApiContractService ethApiContractService,
        string[] contractAddresses)
        {
            return new LogsProcessorBuilder(ethApiContractService, contractAddresses);
        }

        public static LogsProcessorBuilder<TEventDto> LogsProcessor<TEventDto>
            (this IEthApiContractService ethApiContractService)
            where TEventDto : class, IEventDTO, new()
        {
            return new LogsProcessorBuilder<TEventDto>(ethApiContractService);
        }

        public static LogsProcessorBuilder LogsProcessor<TEventDto>
            (this IEthApiContractService ethApiContractService,
            string[] contractAddresses)
            where TEventDto : class, IEventDTO, new()
        {
            return new LogsProcessorBuilder<TEventDto>(ethApiContractService, contractAddresses);
        }

        public static LogsProcessorBuilder LogsProcessor<TEventDto>
            (this IEthApiContractService ethApiContractService,
            string[] contractAddresses,
            Func<IEnumerable<EventLog<TEventDto>>, Task> callBack)
            where TEventDto : class, IEventDTO, new()
        {
            return new LogsProcessorBuilder<TEventDto>(ethApiContractService, contractAddresses).Add(callBack);
        }

        public static LogsProcessorBuilder<TEventDto> LogsProcessor<TEventDto>
            (this IEthApiContractService ethApiContractService,
            string contractAddress,
            Func<IEnumerable<EventLog<TEventDto>>, Task> callBack
            )
            where TEventDto : class, IEventDTO, new()
        {
            return new LogsProcessorBuilder<TEventDto>(ethApiContractService, contractAddress).OnEvents(callBack);
        }


        public static LogsProcessorBuilder<TEventDto> LogsProcessor<TEventDto>
            (this IEthApiContractService ethApiContractService,
            Func<IEnumerable<EventLog<TEventDto>>, Task> callBack
            )
            where TEventDto : class, IEventDTO, new()
        {
            return new LogsProcessorBuilder<TEventDto>(ethApiContractService).OnEvents(callBack);
        }

        public static LogsProcessorBuilder<TEventDto> LogsProcessor<TEventDto>
            (this IEthApiContractService ethApiContractService,
            string[] contractAddresses,
            Action<IEnumerable<EventLog<TEventDto>>> callBack)
            where TEventDto : class, IEventDTO, new() 
        {
            return new LogsProcessorBuilder<TEventDto>(ethApiContractService, contractAddresses).OnEvents(callBack);
        }

        public static LogsProcessorBuilder<TEventDto> LogsProcessor<TEventDto>
            (this IEthApiContractService ethApiContractService,
            string contractAddress,
            Action<IEnumerable<EventLog<TEventDto>>> callBack
            )
            where TEventDto : class, IEventDTO, new()
        {
            return new LogsProcessorBuilder<TEventDto>(ethApiContractService, contractAddress).OnEvents(callBack);
        }

        public static LogsProcessorBuilder<TEventDto> LogsProcessor<TEventDto>
            (this IEthApiContractService ethApiContractService,
            string contractAddress
            )
            where TEventDto : class, IEventDTO, new()
        {
            return new LogsProcessorBuilder<TEventDto>(ethApiContractService, contractAddress);
        }

        public static LogsProcessorBuilder<TEventDto> LogsProcessor<TEventDto>
            (this IEthApiContractService ethApiContractService,
            Action<IEnumerable<EventLog<TEventDto>>> callBack
            )
            where TEventDto : class, IEventDTO, new()
        {
            return new LogsProcessorBuilder<TEventDto>(ethApiContractService).OnEvents(callBack);
        }



    }
}
