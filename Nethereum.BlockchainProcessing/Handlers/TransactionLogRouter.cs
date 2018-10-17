using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Nethereum.BlockchainProcessing.Handlers
{
    /// <summary>
    /// Conditionally route transaction logs to one or many handlers
    /// </summary>
    public class TransactionLogRouter : ITransactionLogHandler
    {
        private readonly List<
            (Func<TransactionLogWrapper, Task<bool>> condition, ITransactionLogHandler handler)> _handlers = 
            new List<(Func<TransactionLogWrapper, Task<bool>> condition, ITransactionLogHandler handler)>();

        public void AddLogHandler(ITransactionLogHandler handler)
        {
            AddLogHandler((log) => true, handler);
        }

        public void AddEventHandler<TEvent>(ITransactionLogHandler<TEvent> handler) 
            where TEvent: new()
        {
            AddLogHandler((log) => log.IsForEvent<TEvent>(), handler);
        }

        public void AddEventHandler<TEvent>(
            Func<TransactionLogWrapper, bool> condition, 
            ITransactionLogHandler<TEvent> handler) where TEvent: new()
        {
            AddLogHandler((log) => log.IsForEvent<TEvent>() && condition(log), handler);
        }

        public void AddEventHandler<TEvent>(
            Func<TransactionLogWrapper, Task<bool>> condition, 
            ITransactionLogHandler<TEvent> handler) where TEvent: new()
        {
            AddLogHandler(async (log) => log.IsForEvent<TEvent>() && await condition(log), handler);
        }

        public void AddLogHandler(
            Func<TransactionLogWrapper, Task<bool>> condition, 
            ITransactionLogHandler handler)
        {
            _handlers.Add((condition, handler));
        }

        public void AddLogHandler(
            Func<TransactionLogWrapper, bool> condition, 
            ITransactionLogHandler handler)
        {
            AddLogHandler(t => Task.FromResult(condition(t)), handler);
        }

        public void AddLogHandler(string toAddress, ITransactionLogHandler handler)
        {
            AddLogHandler(t => 
                t.IsTo(toAddress), 
                handler);
        }

        public void MapToAddresses(IEnumerable<string> toAddresses, ITransactionLogHandler handler)
        {
            foreach (var address in toAddresses)
            {
                AddLogHandler(address, handler);
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
