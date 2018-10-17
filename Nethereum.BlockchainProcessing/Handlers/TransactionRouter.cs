using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Nethereum.Contracts;

namespace Nethereum.BlockchainProcessing.Handlers
{
    /// <summary>
    /// Conditionally route transactions to one or many handlers
    /// </summary>
    public class TransactionRouter : ITransactionHandler
    {
        private int _contractsCreated;
        private int _transactionsHandled;

        public int TransactionsHandled => _transactionsHandled;
        public int ContractsCreated => _contractsCreated;

        private readonly List<
            (Func<TransactionWithReceipt, Task<bool>> condition, ITransactionHandler handler)> _transactionHandlers = 
            new List<(Func<TransactionWithReceipt, Task<bool>> condition, ITransactionHandler handler)>();

        private readonly List<
            (Func<ContractCreationTransaction, Task<bool>> condition, ITransactionHandler handler)> _contractCreationHandlers = 
            new List<(Func<ContractCreationTransaction, Task<bool>> condition, ITransactionHandler handler)>();

        public void AddFunctionHandler<TFunctionMessage>(ITransactionHandler<TFunctionMessage> handler) 
            where TFunctionMessage : FunctionMessage, new()
        {
            AddTransactionHandler((txn) => txn.IsForFunction<TFunctionMessage>(), handler);
        }

        public void AddFunctionHandler<TFunctionMessage>(
            Func<TransactionWithReceipt, bool> condition, 
            ITransactionHandler<TFunctionMessage> handler) 
            where TFunctionMessage : FunctionMessage, new()
        {
            AddTransactionHandler((txn) => 
                txn.IsForFunction<TFunctionMessage>() && condition(txn), handler);
        }

        public void AddFunctionHandler<TFunctionMessage>(
            Func<TransactionWithReceipt, Task<bool>> condition, 
            ITransactionHandler<TFunctionMessage> handler) 
            where TFunctionMessage : FunctionMessage, new()
        {
            AddTransactionHandler(async (txn) => 
                    txn.IsForFunction<TFunctionMessage>() && 
                    await condition(txn), 
                handler);
        }

        public void AddTransactionHandler(ITransactionHandler handler)
        {
            AddTransactionHandler((txn) => true, handler);
        }

        public void AddTransactionHandler(
            Func<TransactionWithReceipt, Task<bool>> condition, 
            ITransactionHandler handler)
        {
            _transactionHandlers.Add((condition, handler));
        }

        public void AddTransactionHandler(
            Func<TransactionWithReceipt, bool> condition, 
            ITransactionHandler handler)
        {
            AddTransactionHandler(t => Task.FromResult(condition(t)), handler);
        }

        public void AddContractCreationHandler(ITransactionHandler handler)
        {
            AddContractCreationHandler((tx) => true, handler);
        }

        public void AddContractCreationHandler(
            Func<ContractCreationTransaction, Task<bool>> condition, 
            ITransactionHandler handler)
        {
            _contractCreationHandlers.Add((condition, handler));
        }

        public void AddContractCreationHandler(
            Func<ContractCreationTransaction, bool> condition, 
            ITransactionHandler handler)
        {
            AddContractCreationHandler(t => Task.FromResult(condition(t)), handler);
        }

        public async Task HandleContractCreationTransactionAsync(
            ContractCreationTransaction contractCreationTransaction)
        {
            foreach (var (condition, handler) in _contractCreationHandlers)
            {
                if (await condition(contractCreationTransaction))
                {
                    await handler.HandleContractCreationTransactionAsync(contractCreationTransaction);
                    Interlocked.Increment(ref _contractsCreated);
                }
            }
        }

        public async Task HandleTransactionAsync(
            TransactionWithReceipt transactionWithReceipt)
        {
            foreach (var (condition, handler) in _transactionHandlers)
            {
                if (await condition(transactionWithReceipt))
                {
                    await handler.HandleTransactionAsync(transactionWithReceipt);
                    Interlocked.Increment(ref _transactionsHandled);
                }
            }
        }
    }
}
