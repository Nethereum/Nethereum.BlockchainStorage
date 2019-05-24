using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.BlockchainProcessing.BlockchainProxy;
using Nethereum.BlockchainProcessing.Processing.Logs.Handling;
using Nethereum.Contracts;
using Nethereum.RPC.Eth.DTOs;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Nethereum.BlockchainProcessing.Processing.Logs
{
    public interface IEventLogProcessor : IDisposable
    {
        event EventHandler OnDisposing;
        
        IBlockchainProxyService BlockchainProxyService { get; set; }
        IBlockProgressRepository BlockProgressRepository { get; set; }
        string[] ContractAddresses { get; }
        Action<Exception> FatalErrorCallback { get; set; }
        List<NewFilterInput> Filters { get; }
        uint? MaximumBlocksPerBatch { get; set; }
        uint? MinimumBlockConfirmations { get; set; }
        ulong? MinimumBlockNumber { get; set; }
        List<ILogProcessor> Processors { get; }
        Action<uint, BlockRange> RangesProcessedCallback { get; set; }

        IEventLogProcessor Configure(Action<IEventLogProcessor> configAction);
        IEventLogProcessor Filter(NewFilterInput filter);
        IEventLogProcessor Filter<TEventDto>() where TEventDto : class, IEventDTO, new();
        IEventLogProcessor OnBatchProcessed(Action<uint, BlockRange> rangesProcessedCallback);
        IEventLogProcessor OnFatalError(Action<Exception> callBack);
        Task<ulong> RunAsync(CancellationToken cancellationToken);
        Task<BlockRange?> RunOnceAsync(CancellationToken cancellationToken);
        Task<Task> RunInBackgroundAsync(CancellationToken cancellationToken);
        IEventLogProcessor CatchAll(Action<IEnumerable<FilterLog>> callBack);
        IEventLogProcessor CatchAll(Func<IEnumerable<FilterLog>, Task> callBack);
        IEventLogProcessor CatchAllAndQueue(IQueue queue, Predicate<FilterLog> predicate = null, Func<FilterLog, object> mapper = null);
        IEventLogProcessor Subscribe(ILogProcessor processor);
        IEventLogProcessor Subscribe<TEventDto>(Action<IEnumerable<EventLog<TEventDto>>> callBack) where TEventDto : class, new();
        IEventLogProcessor Subscribe<TEventDto>(Func<IEnumerable<EventLog<TEventDto>>, Task> callBack) where TEventDto : class, new();
        IEventLogProcessor Subscribe(EventSubscription eventSubscription);
        IEventLogProcessor Subscribe<TEventDto>(EventSubscription<TEventDto> eventSubscription) where TEventDto : class, new();
        IEventLogProcessor SubscribeAndQueue<TEventDto>(IQueue queue, Predicate<EventLog<TEventDto>> predicate = null, Func<EventLog<TEventDto>, object> mapper = null) where TEventDto : class, new();
        IEventLogProcessor UseBlockProgressRepository(IBlockProgressRepository repo);
        IEventLogProcessor UseJsonFileForBlockProgress(string jsonFilePath, bool deleteExistingFile = false);
    }

}
