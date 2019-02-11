using Nethereum.BlockchainProcessing.Handlers;
using Nethereum.Contracts;
using Nethereum.Contracts.Extensions;
using Nethereum.RPC.Eth.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nethereum.BlockchainStore.Search
{
    public class FunctionIndexTransactionHandler<TFunctionMessage> : 
        IDisposable, 
        ITransactionHandler<TFunctionMessage> 
        where TFunctionMessage : FunctionMessage, new()
    {
        private readonly int _logsPerIndexBatch;
        private Queue<FunctionCall<TFunctionMessage>> _currentBatch = new Queue<FunctionCall<TFunctionMessage>>();
        private readonly IFunctionIndexer<TFunctionMessage> _functionIndexer;

        public FunctionIndexTransactionHandler(IFunctionIndexer<TFunctionMessage> functionIndexer, int logsPerIndexBatch = 1)
        {
            _functionIndexer = functionIndexer;
            _logsPerIndexBatch = logsPerIndexBatch;
        }

        public void Dispose()
        {
            if (_currentBatch.Any())
            {
                _functionIndexer.IndexAsync(_currentBatch).Wait();
                _currentBatch.Clear();
            }
        }

        public int Pending => _currentBatch.Count;

        public Task HandleContractCreationTransactionAsync(ContractCreationTransaction contractCreationTransaction)
        {
            return Task.CompletedTask;
        }

        public async Task HandleTransactionAsync(TransactionWithReceipt transactionWithReceipt)
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
                    await _functionIndexer.IndexAsync(_currentBatch);
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
