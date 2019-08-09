using System;
using System.Threading.Tasks;
using Nethereum.BlockchainStore.Search;
using Nethereum.Contracts;
using Nethereum.RPC.Eth.DTOs;

namespace Nethereum.BlockchainProcessing.Processor
{
    public class TransactionReceiptVOProcessorHandler<TFunctionMessage> : ProcessorHandler<TransactionReceiptVO> 
        where TFunctionMessage : FunctionMessage, new()
    {
        Func<TransactionForFunctionVO<TFunctionMessage>, Task> _action;
        Func<TransactionForFunctionVO<TFunctionMessage>, Task<bool>> _criteria;

        public TransactionReceiptVOProcessorHandler(
            Func<TransactionForFunctionVO<TFunctionMessage>, Task> action) : this(action, null)
        {
        }

        public TransactionReceiptVOProcessorHandler(
            Func<TransactionForFunctionVO<TFunctionMessage>, Task> action,
            Func<TransactionForFunctionVO<TFunctionMessage>, Task<bool>> eventCriteria)
        {
            _action = action;
            _criteria = eventCriteria;
            SetMatchCriteriaForFunction();
        }

        public TransactionReceiptVOProcessorHandler(
            Action<TransactionForFunctionVO<TFunctionMessage>> action) : this(action, null)
        {
        }

        public TransactionReceiptVOProcessorHandler(
            Action<TransactionForFunctionVO<TFunctionMessage>> action,
            Func<TransactionForFunctionVO<TFunctionMessage>, bool> criteria)
        {
            _action = (l) => { action(l); return Task.FromResult(0); };
            if (criteria != null)
            {
                _criteria = async (l) => { return await Task.FromResult(criteria(l)); };
            }
            SetMatchCriteriaForFunction();
        }

        private void SetMatchCriteriaForFunction()
        {
            base.SetMatchCriteria(async transaction =>
            {
                if (await Task.FromResult(transaction.IsTransactionForFunctionMessage<TFunctionMessage>()) == false) return false;

                if (_criteria == null) return true;

                var functionCall = new TransactionForFunctionVO<TFunctionMessage>(transaction, transaction.Decode<TFunctionMessage>());

                return await _criteria(functionCall).ConfigureAwait(false);
            });
        }

        protected override Task ExecuteInternalAsync(TransactionReceiptVO value)
        {
            var functionCall = new TransactionForFunctionVO<TFunctionMessage>(value, value.Decode<TFunctionMessage>());
            return _action(functionCall);
        }
    }
}