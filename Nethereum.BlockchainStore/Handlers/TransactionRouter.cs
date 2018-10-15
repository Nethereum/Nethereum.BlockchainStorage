using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Nethereum.BlockchainStore.Handlers
{
    /// <summary>
    /// Conditionally route transactions to one or many handlers
    /// </summary>
    public class TransactionRouter : ITransactionHandler
    {
        private readonly List<
            (Func<TransactionWithReceipt, Task<bool>> condition, ITransactionHandler handler)> _handlers = 
            new List<(Func<TransactionWithReceipt, Task<bool>> condition, ITransactionHandler handler)>();

        private readonly List<
            (Func<ContractCreationTransaction, Task<bool>> condition, ITransactionHandler handler)> _contractCreationHandlers = 
            new List<(Func<ContractCreationTransaction, Task<bool>> condition, ITransactionHandler handler)>();

        public void Add(ITransactionHandler handler)
        {
            Map((txn) => true, handler);
        }

        public void Map(Func<TransactionWithReceipt, Task<bool>> condition, ITransactionHandler handler)
        {
            _handlers.Add((condition, handler));
        }

        public void Map(Func<TransactionWithReceipt, bool> condition, ITransactionHandler handler)
        {
            Map(t => Task.FromResult(condition(t)), handler);
        }

        public void MapContractCreation(Func<ContractCreationTransaction, Task<bool>> condition, ITransactionHandler handler)
        {
            _contractCreationHandlers.Add((condition, handler));
        }

        public void MapContractCreation(Func<ContractCreationTransaction, bool> condition, ITransactionHandler handler)
        {
            MapContractCreation(t => Task.FromResult(condition(t)), handler);
        }

        public async Task HandleContractCreationTransactionAsync(ContractCreationTransaction contractCreationTransaction)
        {
            foreach (var (condition, handler) in _contractCreationHandlers)
            {
                if (await condition(contractCreationTransaction))
                {
                    await handler.HandleContractCreationTransactionAsync(contractCreationTransaction);
                }
            }
        }

        public async Task HandleTransactionAsync(TransactionWithReceipt transactionWithReceipt)
        {
            foreach (var (condition, handler) in _handlers)
            {
                if (await condition(transactionWithReceipt))
                {
                    await handler.HandleTransactionAsync(transactionWithReceipt);
                }
            }
        }
    }
}
