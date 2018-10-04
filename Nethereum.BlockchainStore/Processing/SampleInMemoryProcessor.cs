using Nethereum.BlockchainStore.Handlers;
using Nethereum.Hex.HexTypes;
using Nethereum.RPC.Eth.DTOs;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Nethereum.BlockchainStore.Processing
{
    public class SampleInMemoryProcessor
    {
        public class InMemoryHandlerBase
        {
            private readonly Action<string> logAction;

            protected virtual void Log(string message)
            {
                logAction(message);
            }

            protected InMemoryHandlerBase(Action<string> logAction)
            {
                this.logAction = logAction;
            }
        }

        public class InMemoryBlockHandler : InMemoryHandlerBase, IBlockHandler
        {
            public InMemoryBlockHandler(Action<string> logAction) : base(logAction)
            {
            }

            public Task HandleAsync(BlockWithTransactionHashes block)
            {
                Log($"Block: {block.Number.Value}");
                return Task.CompletedTask;
            }
        }

        public class InMemoryTransactionHandler : InMemoryHandlerBase, ITransactionHandler
        {
            public InMemoryTransactionHandler(Action<string> logAction) : base(logAction)
            {
            }

            public Task HandleAddressTransactionAsync(Transaction transaction, TransactionReceipt transactionReceipt, bool hasError, HexBigInteger blockTimestamp, string address, string error = null, bool hasVmStack = false)
            {
                Log($"[AddressTransaction] Block:{transaction.BlockNumber.Value}, Index:{transaction.TransactionIndex.Value}, Hash:{transaction.TransactionHash}, Address:{address}, From:{transaction.From}, To:{transaction.To}");
                return Task.CompletedTask;
            }

            public Task HandleContractCreationTransactionAsync(string contractAddress, string code, Transaction transaction, TransactionReceipt transactionReceipt, bool failedCreatingContract, HexBigInteger blockTimestamp)
            {
                Log($"[ContractCreation] Block:{transaction.BlockNumber.Value}, Index:{transaction.TransactionIndex.Value}, Hash:{transaction.TransactionHash}, Contract:{contractAddress}, From:{transaction.From}, To:{transaction.To}");
                return Task.CompletedTask;
            }

            public Task HandleTransactionAsync(Transaction transaction, TransactionReceipt transactionReceipt, bool hasError, HexBigInteger blockTimestamp, string error = null, bool hasVmStack = false)
            {
                Log($"[Transaction] Block:{transaction.BlockNumber.Value}, Index:{transaction.TransactionIndex.Value}, Hash:{transaction.TransactionHash}, From:{transaction.From}, To:{transaction.To}");
                return Task.CompletedTask;
            }
        }

        public class InMemoryTransactionLogHandler : InMemoryHandlerBase, ITransactionLogHandler
        {
            public InMemoryTransactionLogHandler(Action<string> logAction) : base(logAction)
            {
            }

            public Task HandleAsync(string transactionHash, long logIndex, JObject log)
            {
                Log($"[TransactionLog] Hash:{transactionHash}, Index:{logIndex}, Address:{log["address"]}");
                return Task.CompletedTask;
            }
        }

        public class InMemoryTransactionVmStackHandler : InMemoryHandlerBase, ITransactionVMStackHandler
        {
            public InMemoryTransactionVmStackHandler(Action<string> logAction) : base(logAction)
            {
            }

            public Task HandleAsync(string transactionHash, string address, JObject stackTrace)
            {
                Log($"[TransactionVmStack] Hash:{transactionHash}, Address:{address}");
                return Task.CompletedTask;
            }
        }

        public class InMemoryContractHandler : InMemoryHandlerBase, IContractHandler
        {
            readonly HashSet<string> _cachedContracts = new HashSet<string>();

            public InMemoryContractHandler(Action<string> logAction) : base(logAction)
            {
            }

            public Task<bool> ExistsAsync(string contractAddress)
            {
                var isCached = _cachedContracts.Contains(contractAddress);
                if (isCached)
                {
                    Log($"[Contract Cache Hit] {contractAddress}");
                }
                
                return Task.FromResult(isCached);
            }

            public Task HandleAsync(string contractAddress, string code, Transaction transaction)
            {
                Log($"[Contract Add] Block:{transaction.BlockNumber.Value}, Hash:{transaction.TransactionHash}, Contract:{contractAddress}, Sender:{transaction.From}");
                _cachedContracts.Add(contractAddress);
                return  Task.CompletedTask;
            }
        }

        public class InMemoryProcessingStrategy : IBlockchainProcessingStrategy
        {
            public FilterContainer Filters { get; }
            public IBlockHandler BlockHandler { get; }
            public ITransactionHandler TransactionHandler { get; }
            public ITransactionLogHandler TransactionLogHandler { get; }
            public ITransactionVMStackHandler TransactionVmStackHandler { get; }
            public IContractHandler ContractHandler { get; }
            public int MaxRetries => 3;
            public long MinimumBlockNumber => 0;


            public InMemoryProcessingStrategy(Action<string> logAction, FilterContainer filters = null)
            {
                Filters = filters;
                BlockHandler = new InMemoryBlockHandler(logAction);
                TransactionHandler = new InMemoryTransactionHandler(logAction);
                TransactionLogHandler = new InMemoryTransactionLogHandler(logAction);
                TransactionVmStackHandler = new InMemoryTransactionVmStackHandler(logAction);
                ContractHandler = new InMemoryContractHandler(logAction);
            }

            public void Dispose()
            {
            }

            public Task FillContractCacheAsync(){ return Task.CompletedTask;}

            public Task<long> GetLastBlockProcessedAsync() => Task.FromResult((long)0);

            public Task PauseFollowingAnError(int retryNumber) => Task.Delay(1000);

            public Task WaitForNextBlock(int retryNumber) => Task.Delay(1000);
        }
    }
}
