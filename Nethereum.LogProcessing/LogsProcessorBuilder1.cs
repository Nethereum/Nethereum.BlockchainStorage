using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.BlockchainProcessing.BlockchainProxy;
using Nethereum.Contracts;
using Nethereum.Contracts.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nethereum.BlockchainProcessing.Processing.Logs
{
    public class LogsProcessorBuilder<TEventDto> : LogsProcessorBuilder, ILogsProcessorBuilder<TEventDto> where TEventDto : class, IEventDTO, new()
    {
        public LogsProcessorBuilder(string blockchainUrl):this(new Web3.Web3(blockchainUrl).Eth)
        {
        }

        public LogsProcessorBuilder(string blockchainUrl, string contractAddress) : this(new Web3.Web3(blockchainUrl).Eth, contractAddress)
        {
        }

        public LogsProcessorBuilder(string blockchainUrl, string[] contractAddresses) : this(new Web3.Web3(blockchainUrl).Eth, contractAddresses)
        {
        }

        public LogsProcessorBuilder(string blockchainUrl, Action<NewFilterInputBuilder<TEventDto>> configureFilterBuilder) : this(new Web3.Web3(blockchainUrl).Eth, configureFilterBuilder)
        {
        }

        public LogsProcessorBuilder(string blockchainUrl, string contractAddress, Action<NewFilterInputBuilder<TEventDto>> configureFilterBuilder) : this(new Web3.Web3(blockchainUrl).Eth, configureFilterBuilder, contractAddress)
        {
        }

        public LogsProcessorBuilder(string blockchainUrl, string[] contractAddresses, Action<NewFilterInputBuilder<TEventDto>> configureFilterBuilder) : this(new Web3.Web3(blockchainUrl).Eth, configureFilterBuilder, contractAddresses)
        {
        }

        public LogsProcessorBuilder(IEthApiContractService ethApiContractService)
            : base(ethApiContractService, new NewFilterInputBuilder<TEventDto>().Build())
        {
        }

        public LogsProcessorBuilder(IEthApiContractService ethApiContractService, Action<NewFilterInputBuilder<TEventDto>> configureFilterBuilder, params string[] contractAddresses)
            : base(ethApiContractService)
        {
            var filterBuilder = new NewFilterInputBuilder<TEventDto>();
            configureFilterBuilder(filterBuilder);
            Filters.Add(filterBuilder.Build(contractAddresses));
        }

        public LogsProcessorBuilder(IEthApiContractService ethApiContractService, string contractAddress)
            : base(ethApiContractService, new NewFilterInputBuilder<TEventDto>().Build(contractAddress))
        {
        }

        public LogsProcessorBuilder(IEthApiContractService ethApiContractService, string[] contractAddresses)
            : base(ethApiContractService, new NewFilterInputBuilder<TEventDto>().Build(contractAddresses))
        {
        }

        public ILogsProcessorBuilder<TEventDto> OnEvents(Action<IEnumerable<EventLog<TEventDto>>> callBack)
        {
            var asyncCallback = new Func<IEnumerable<EventLog<TEventDto>>, Task>(async (events) => await Task.Run(() => callBack(events)).ConfigureAwait(false));
            return OnEvents(asyncCallback);
        }

        public ILogsProcessorBuilder<TEventDto> OnEvents(Func<IEnumerable<EventLog<TEventDto>>, Task> callBack)
        {
            Processors.Add(new LogProcessor<TEventDto>(callBack));
            return this;
        }
    }
}
