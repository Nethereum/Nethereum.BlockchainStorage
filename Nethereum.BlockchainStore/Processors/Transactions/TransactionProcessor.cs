using Nethereum.BlockchainStore.Web3Abstractions;
using Nethereum.RPC.Eth.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Nethereum.BlockchainStore.Processors.Transactions
{
    public class TransactionProcessor : ITransactionProcessor
    {
        protected readonly ITransactionProxy TransactionProxy;
        private readonly IContractTransactionProcessor _contractTransactionProcessor;
        private readonly IValueTransactionProcessor _valueTransactionProcessor;
        private readonly IContractCreationTransactionProcessor _contractCreationTransactionProcessor;
        private readonly List<ITransactionFilter> _transactionFilters;
        private readonly List<ITransactionReceiptFilter> _transactionReceiptFilters;

        public bool EnabledContractCreationProcessing { get; set; } = true;
        public bool EnabledContractProcessing { get; set; } = true;
        public bool EnabledValueProcessing { get; set; } = true;
        public IContractTransactionProcessor ContractTransactionProcessor => _contractTransactionProcessor;

        public TransactionProcessor(
            ITransactionProxy transactionProxy, 
            IContractTransactionProcessor contractTransactionProcessor, 
            IValueTransactionProcessor valueTransactionProcessor, 
            IContractCreationTransactionProcessor contractCreationTransactionProcessor,
            IEnumerable<ITransactionFilter> transactionFilters = null,
            IEnumerable<ITransactionReceiptFilter> transactionReceiptFilters = null)
        {
            TransactionProxy = transactionProxy;
            _contractTransactionProcessor = contractTransactionProcessor;
            _valueTransactionProcessor = valueTransactionProcessor;
            _contractCreationTransactionProcessor = contractCreationTransactionProcessor;
            _transactionFilters = new List<ITransactionFilter>(transactionFilters ?? new ITransactionFilter[0]);
            _transactionReceiptFilters = new List<ITransactionReceiptFilter>(transactionReceiptFilters ?? new ITransactionReceiptFilter[0]);
        }
       
        public virtual async Task ProcessTransactionAsync(string transactionHash, BlockWithTransactionHashes block)
        {
            var transactionSource = await GetTransaction(transactionHash).ConfigureAwait(false);

            if (await _transactionFilters.IgnoreAsync(transactionSource)) return;

            var transactionReceipt = await GetTransactionReceipt(transactionHash).ConfigureAwait(false);

            if (await _transactionReceiptFilters.IgnoreAsync(transactionReceipt)) return;
            
            if ( transactionSource.IsForContractCreation(transactionReceipt))
            {
                if (EnabledContractCreationProcessing)
                {
                    await
                        _contractCreationTransactionProcessor.ProcessTransactionAsync(transactionSource, transactionReceipt,
                            block.Timestamp).ConfigureAwait(false);
                }
            }
            else
            {
                if (await _contractTransactionProcessor.IsTransactionForContractAsync(transactionSource))
                {
                    if (EnabledContractProcessing)
                    {
                        await
                            _contractTransactionProcessor.ProcessTransactionAsync(transactionSource, transactionReceipt,
                                block.Timestamp).ConfigureAwait(false);
                    }
                }
                else if (EnabledValueProcessing)
                {
                    await
                        _valueTransactionProcessor.ProcessTransactionAsync(transactionSource, transactionReceipt,
                            block.Timestamp).ConfigureAwait(false);
                }
            }
        }

        protected virtual async Task<Transaction> GetTransaction(string txnHash)
        {
            return await TransactionProxy.GetTransactionByHash(txnHash).ConfigureAwait(false);
        }

        protected virtual async Task<TransactionReceipt> GetTransactionReceipt(string txnHash)
        {
            return await TransactionProxy.GetTransactionReceipt(txnHash).ConfigureAwait(false);
        }
    }
}