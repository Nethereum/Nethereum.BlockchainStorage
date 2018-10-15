using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Nethereum.BlockchainStore.Handlers
{
    /// <summary>
    /// Conditionally route transaction logs to one or many handlers
    /// </summary>
    public class TransactionLogRouter : ITransactionLogHandler
    {
        private readonly List<
            (Func<TransactionLogWrapper, Task<bool>> condition, ITransactionLogHandler handler)> _handlers = 
            new List<(Func<TransactionLogWrapper, Task<bool>> condition, ITransactionLogHandler handler)>();

        public void Add(ITransactionLogHandler handler)
        {
            Map((log) => true, handler);
        }

        public void Map(Func<TransactionLogWrapper, Task<bool>> condition, ITransactionLogHandler handler)
        {
            _handlers.Add((condition, handler));
        }

        public void Map(Func<TransactionLogWrapper, bool> condition, ITransactionLogHandler handler)
        {
            Map(t => Task.FromResult(condition(t)), handler);
        }

        public void MapToAddress(string toAddress, ITransactionLogHandler handler)
        {
            Map(t => 
                toAddress.Equals(t.Transaction.To, StringComparison.InvariantCultureIgnoreCase), 
                handler);
        }

        public void MapToAddresses(IEnumerable<string> toAddresses, ITransactionLogHandler handler)
        {
            foreach (var address in toAddresses)
            {
                MapToAddress(address, handler);
            }
        }

        public async Task HandleAsync(TransactionLogWrapper transactionLog)
        {
            foreach (var (condition, handler) in _handlers)
            {
                if (await condition(transactionLog))
                {
                    await handler.HandleAsync(transactionLog);
                }                    
            }
        }

    }
}
