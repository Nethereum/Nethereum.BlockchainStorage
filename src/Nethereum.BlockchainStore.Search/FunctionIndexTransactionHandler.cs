using Nethereum.BlockchainProcessing.Handlers;
using Nethereum.Contracts;
using Nethereum.Contracts.Extensions;
using Nethereum.RPC.Eth.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Nethereum.BlockProcessing.ValueObjects;

namespace Nethereum.BlockchainStore.Search
{
    /// <summary>
    /// Decodes function from transaction, ensures the tx does relate to the function and sends to indexer
    /// </summary>
    /// <typeparam name="TFunctionMessage">The Function Message DTO describing the signature of the function</typeparam>
    public class FunctionIndexTransactionHandler<TFunctionMessage> :         
        IDisposable,
        ITransactionHandler<TFunctionMessage>
        where TFunctionMessage : FunctionMessage, new()
    {
        private readonly int _logsPerIndexBatch;
        private readonly Queue<FunctionCall<TFunctionMessage>> _currentBatch = new Queue<FunctionCall<TFunctionMessage>>();

        public FunctionIndexTransactionHandler(IFunctionIndexer<TFunctionMessage> functionIndexer, int logsPerIndexBatch = 1)
        {
            FunctionIndexer = functionIndexer;
            _logsPerIndexBatch = logsPerIndexBatch;
        }

        public void Dispose()
        {
            if (_currentBatch.Any())
            {
                FunctionIndexer.IndexAsync(_currentBatch).Wait();
                _currentBatch.Clear();
            }
        }

        public int Pending => _currentBatch.Count;

        public IFunctionIndexer<TFunctionMessage> FunctionIndexer { get; }

        public Task HandleContractCreationTransactionAsync(
            ContractCreationTransaction contractCreationTransaction) => HandleAsync(contractCreationTransaction);

        public Task HandleTransactionAsync(TransactionWithReceipt transactionWithReceipt) =>
            HandleAsync(transactionWithReceipt);

        private async Task HandleAsync(TransactionWithReceipt transactionWithReceipt)
        {
            try
            {
                if (!transactionWithReceipt.IsForFunction<TFunctionMessage>()) return;

                var decoded = transactionWithReceipt.Decode<TFunctionMessage>();
                if (decoded == null)
                {
                    return;
                }

                _currentBatch.Enqueue(new FunctionCall<TFunctionMessage>(transactionWithReceipt, decoded));
                if (_currentBatch.Count == _logsPerIndexBatch)
                {
                    await FunctionIndexer.IndexAsync(_currentBatch);
                    _currentBatch.Clear();
                }
            }
            catch (Exception)
            {
                //Error whilst handling transaction log
                //expected event signature may differ from the expected event.   
            }
        }
    }
}
