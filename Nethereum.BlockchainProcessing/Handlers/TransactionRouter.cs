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

        public void AddTransactionHandler(
            ITransactionHandler handler)
        {
            _AddTransactionHandler((txn) => true, handler);
        }

        public void AddTransactionHandler(
            Func<TransactionWithReceipt, bool> condition, 
            ITransactionHandler handler)
        {
            _AddTransactionHandler(condition, handler);
        }

        public void AddTransactionHandler(
            Func<TransactionWithReceipt, Task<bool>> condition, 
            ITransactionHandler handler)
        {
            _AddTransactionHandler(condition, handler);
        }

        public void AddTransactionHandler<TFunctionMessage>(
            ITransactionHandler<TFunctionMessage> handler) 
            where TFunctionMessage: FunctionMessage, new()
        {
            var wrappedCondition = new Func<TransactionWithReceipt, bool>(
                txn => txn.IsForFunction<TFunctionMessage>());

            _AddTransactionHandler(wrappedCondition, handler);
        }

        public void AddTransactionHandler<TFunctionMessage>(
            Func<TransactionWithReceipt, bool> condition,
            ITransactionHandler<TFunctionMessage> handler) 
            where TFunctionMessage: FunctionMessage, new()
        {
            var wrappedCondition = new Func<TransactionWithReceipt, bool>(
                txn => txn.IsForFunction<TFunctionMessage>() && condition(txn));

            _AddTransactionHandler(wrappedCondition, handler);
        }

        public void AddTransactionHandler<TFunctionMessage>(
            Func<TransactionWithReceipt, Task<bool>> condition,
            ITransactionHandler<TFunctionMessage> handler) 
            where TFunctionMessage: FunctionMessage, new()
        {
            var wrappedCondition = new Func<TransactionWithReceipt, Task<bool>>(
                async txn => txn.IsForFunction<TFunctionMessage>() && await condition(txn));

            _AddTransactionHandler(wrappedCondition, handler);
        }

        private void _AddTransactionHandler(Func<TransactionWithReceipt, bool> condition,
            ITransactionHandler handler)
        {
            _AddTransactionHandler((txn) => Task.FromResult(condition(txn)), handler);
        }

        private void _AddTransactionHandler(Func<TransactionWithReceipt, Task<bool>> condition,
            ITransactionHandler handler)
        {
            _transactionHandlers.Add((condition, handler));
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
                    await handler
                        .HandleContractCreationTransactionAsync(contractCreationTransaction)
                        .ConfigureAwait(false);

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
                    await handler.HandleTransactionAsync(transactionWithReceipt)
                        .ConfigureAwait(false);

                    Interlocked.Increment(ref _transactionsHandled);
                }
            }
        }
    }
}
