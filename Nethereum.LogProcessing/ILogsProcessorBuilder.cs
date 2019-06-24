using Common.Logging;
using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.BlockchainProcessing.Processing;
using Nethereum.BlockchainProcessing.Processing.Logs;
using Nethereum.BlockchainProcessing.Processing.Logs.Handling;
using Nethereum.Contracts;
using Nethereum.Contracts.Services;
using Nethereum.RPC.Eth.DTOs;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Threading.Tasks;

namespace Nethereum.LogProcessing
{
    public interface ILogsProcessorBuilder
    {
        Action<LogBatchProcessedArgs> BatchProcessedCallback { get; set; }
        IEthApiContractService Eth { get; set; }
        IBlockProgressRepository BlockProgressRepository { get; set; }
        uint? BlocksPerBatch { get; set; }
        string[] ContractAddresses { get; }
        Action<Exception> FatalErrorCallback { get; set; }
        List<NewFilterInput> Filters { get; }
        uint? MinimumBlockConfirmations { get; set; }
        BigInteger? MinimumBlockNumber { get; set; }
        List<ILogProcessor> Processors { get; }

        ILog Log { get; set;}

        ILogsProcessorBuilder Add(Action<IEnumerable<FilterLog>> callBack);
        ILogsProcessorBuilder Add(Func<IEnumerable<FilterLog>, Task> callBack);

        ILogsProcessorBuilder Add(Predicate<FilterLog> isItForMe, Action<IEnumerable<FilterLog>> action);

        ILogsProcessorBuilder Add(Predicate<FilterLog> isItForMe, Func<IEnumerable<FilterLog>, Task> func);

        ILogsProcessorBuilder Add(ILogProcessor processor);
        ILogsProcessorBuilder Add<TEventDto>(Action<IEnumerable<EventLog<TEventDto>>> callBack) where TEventDto : class, new();
        ILogsProcessorBuilder Add<TEventDto>(Func<IEnumerable<EventLog<TEventDto>>, Task> callBack) where TEventDto : class, new();
        ILogsProcessor Build();
        ILogsProcessorBuilder Filter(NewFilterInput filter);
        ILogsProcessorBuilder Filter<TEventDto>() where TEventDto : class, IEventDTO, new();
        ILogsProcessorBuilder Filter<TEventDto>(Action<FilterInputBuilder<TEventDto>> configureFilter) where TEventDto : class, IEventDTO, new();

        ILogsProcessorBuilder OnBatchProcessed(Action batchProcessedCallback);
        ILogsProcessorBuilder OnBatchProcessed(Action<LogBatchProcessedArgs> batchProcessedCallback);
        ILogsProcessorBuilder OnFatalError(Action<Exception> callBack);
        ILogsProcessorBuilder DisposeOnProcessorDisposing(IDisposable objectToDisposeWithProcessor);
        ILogsProcessorBuilder Set(Action<ILogsProcessorBuilder> configAction);
        ILogsProcessorBuilder SetBlockProgressRepository(IBlockProgressRepository blockProgressRepository);
        ILogsProcessorBuilder SetBlocksPerBatch(uint blocksPerBatch);
        ILogsProcessorBuilder SetMinimumBlockConfirmations(uint minBlockConfirmations);
        ILogsProcessorBuilder SetMinimumBlockNumber(BigInteger minimumBlockNumber);

        ILogsProcessorBuilder SetLog(ILog log);
        ILogsProcessorBuilder UseBlockProgressRepository(IBlockProgressRepository repo);
        ILogsProcessorBuilder UseJsonFileForBlockProgress(string jsonFilePath, bool deleteExistingFile = false);
    }
}
