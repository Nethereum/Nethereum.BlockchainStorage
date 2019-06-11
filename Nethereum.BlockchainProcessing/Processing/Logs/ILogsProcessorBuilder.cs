using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.BlockchainProcessing.BlockchainProxy;
using Nethereum.BlockchainProcessing.Processing.Logs.Handling;
using Nethereum.Contracts;
using Nethereum.Contracts.Services;
using Nethereum.RPC.Eth.DTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Nethereum.BlockchainProcessing.Processing.Logs
{
    public interface ILogsProcessorBuilder
    {
        Action<uint, BlockRange> BatchProcessedCallback { get; set; }
        IEthApiContractService Eth { get; set; }
        IBlockProgressRepository BlockProgressRepository { get; set; }
        uint? BlocksPerBatch { get; set; }
        string[] ContractAddresses { get; }
        Action<Exception> FatalErrorCallback { get; set; }
        List<NewFilterInput> Filters { get; }
        uint? MinimumBlockConfirmations { get; set; }
        ulong? MinimumBlockNumber { get; set; }
        List<ILogProcessor> Processors { get; }

        ILogsProcessorBuilder Add(Action<IEnumerable<FilterLog>> callBack);
        ILogsProcessorBuilder Add(EventSubscription eventSubscription);
        ILogsProcessorBuilder Add(Func<IEnumerable<FilterLog>, Task> callBack);
        ILogsProcessorBuilder Add(ILogProcessor processor);
        ILogsProcessorBuilder Add<TEventDto>(Action<IEnumerable<EventLog<TEventDto>>> callBack) where TEventDto : class, new();
        ILogsProcessorBuilder Add<TEventDto>(EventSubscription<TEventDto> eventSubscription) where TEventDto : class, new();
        ILogsProcessorBuilder Add<TEventDto>(Func<IEnumerable<EventLog<TEventDto>>, Task> callBack) where TEventDto : class, new();
        ILogsProcessorBuilder AddToQueue(IQueue queue, Predicate<FilterLog> predicate = null, Func<FilterLog, object> mapper = null);
        ILogsProcessorBuilder AddToQueue<TEventDto>(IQueue queue, Predicate<EventLog<TEventDto>> predicate = null, Func<EventLog<TEventDto>, object> mapper = null) where TEventDto : class, new();
        ILogsProcessor Build();
        ILogsProcessorBuilder Filter(NewFilterInput filter);
        ILogsProcessorBuilder Filter<TEventDto>() where TEventDto : class, IEventDTO, new();
        ILogsProcessorBuilder Filter<TEventDto>(Action<NewFilterInputBuilder<TEventDto>> configureFilter) where TEventDto : class, IEventDTO, new();

        ILogsProcessorBuilder OnBatchProcessed(Action batchProcessedCallback);
        ILogsProcessorBuilder OnBatchProcessed(Action<uint, BlockRange> batchProcessedCallback);
        ILogsProcessorBuilder OnFatalError(Action<Exception> callBack);
        ILogsProcessorBuilder DisposeOnProcessorDisposing(IDisposable objectToDisposeWithProcessor);
        ILogsProcessorBuilder Set(Action<ILogsProcessorBuilder> configAction);
        ILogsProcessorBuilder SetBlockProgressRepository(IBlockProgressRepository blockProgressRepository);
        ILogsProcessorBuilder SetBlocksPerBatch(uint blocksPerBatch);
        ILogsProcessorBuilder SetMinimumBlockConfirmations(uint minBlockConfirmations);
        ILogsProcessorBuilder SetMinimumBlockNumber(ulong minimumBlockNumber);
        ILogsProcessorBuilder UseBlockProgressRepository(IBlockProgressRepository repo);
        ILogsProcessorBuilder UseJsonFileForBlockProgress(string jsonFilePath, bool deleteExistingFile = false);
    }
}
