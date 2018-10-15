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


        public void Map(Func<TransactionWithReceipt, Task<bool>> condition, ITransactionHandler handler)
        {
            _handlers.Add((condition, handler));
        }

        public void Map(Func<TransactionWithReceipt, bool> condition, ITransactionHandler handler)
        {
            Map(t => Task.FromResult(condition(t)), handler);
        }

        public void Map(Func<ContractCreationTransaction, Task<bool>> condition, ITransactionHandler handler)
        {
            _contractCreationHandlers.Add((condition, handler));
        }

        public void Map(Func<ContractCreationTransaction, bool> condition, ITransactionHandler handler)
        {
            Map(t => Task.FromResult(condition(t)), handler);
        }

        public async Task HandleContractCreationTransactionAsync(ContractCreationTransaction contractCreationTransaction)
        {
            foreach (var item in _contractCreationHandlers)
            {
                if (await item.condition(contractCreationTransaction))
                {
                    await item.handler.HandleContractCreationTransactionAsync(contractCreationTransaction);
                }
            }
        }

        public async Task HandleTransactionAsync(TransactionWithReceipt transactionWithReceipt)
        {
            foreach (var item in _handlers)
            {
                if (await item.condition(transactionWithReceipt))
                {
                    await item.handler.HandleTransactionAsync(transactionWithReceipt);
                }
            }
        }
    }
}
