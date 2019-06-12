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

        public void AddHandler(ITransactionLogHandler handler)
        {
            _AddHandler((log) => true, handler);
        }

        public void AddHandler(
            Func<TransactionLogWrapper, bool> condition, 
            ITransactionLogHandler handler)
        {
            _AddHandler(condition, handler);
        }

        public void AddHandler(
            Func<TransactionLogWrapper, Task<bool>> condition, 
            ITransactionLogHandler handler)
        {
            _AddHandler(condition, handler);
        }

        public void AddHandler<TEvent>(ITransactionLogHandler<TEvent> handler) 
            where TEvent: new()
        {
            _AddHandler((log) => log.IsForEvent<TEvent>(), handler);
        }

        public void AddHandler<TEvent>(
            Func<TransactionLogWrapper, bool> condition, 
            ITransactionLogHandler<TEvent> handler) where TEvent: new()
        {
            _AddHandler((log) => log.IsForEvent<TEvent>() && condition(log), handler);
        }

        public void AddHandler<TEvent>(
            Func<TransactionLogWrapper, Task<bool>> condition, 
            ITransactionLogHandler<TEvent> handler) where TEvent: new()
        {
            _AddHandler(async (log) => log.IsForEvent<TEvent>() && await condition(log), handler);
        }

        private void _AddHandler(
            Func<TransactionLogWrapper, bool> condition, 
            ITransactionLogHandler handler)
        {
            AddHandler(t => Task.FromResult(condition(t)), handler);
        }

        private void _AddHandler(
            Func<TransactionLogWrapper, Task<bool>> condition, 
            ITransactionLogHandler handler)
        {
            _handlers.Add((condition, handler));
        }

        public async Task HandleAsync(TransactionLogWrapper transactionLog)
        {
            foreach (var (condition, handler) in _handlers)
            {
                if (await condition(transactionLog))
                {
                    await handler.HandleAsync(transactionLog).ConfigureAwait(false);
                }                    
            }
        }

    }
}
