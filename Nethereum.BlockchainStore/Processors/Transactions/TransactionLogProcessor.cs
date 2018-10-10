using Nethereum.BlockchainStore.Handlers;
using Nethereum.RPC.Eth.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Nethereum.BlockchainStore.Processors.Transactions
{
    public class TransactionLogProcessor : ITransactionLogProcessor
    {
        private readonly IEnumerable<ITransactionLogFilter> _filters;
        private readonly ITransactionLogHandler _transactionLogHandler;

        public TransactionLogProcessor(IEnumerable<ITransactionLogFilter> filters, ITransactionLogHandler transactionLogHandler)
        {
            _filters = filters;
            this._transactionLogHandler = transactionLogHandler;
        }

        public async Task ProcessAsync(TransactionReceipt receipt)
        {
            if (receipt?.Logs == null) return;

            foreach (var transactionLog in receipt.GetTransactionLogs())
            {
                if (await _filters.IsMatchAsync(transactionLog.Log))
                {
                    await _transactionLogHandler.HandleAsync(transactionLog);
                }
            }
        }
    }
}
