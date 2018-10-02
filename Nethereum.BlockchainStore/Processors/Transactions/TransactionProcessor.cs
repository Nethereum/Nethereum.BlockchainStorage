using Nethereum.BlockchainStore.Web3Abstractions;
using Nethereum.RPC.Eth.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Nethereum.BlockchainStore.Processors.Transactions
{
    public class TransactionProcessor : ITransactionProcessor
    {
        private readonly IValueTransactionProcessor _valueTransactionProcessor;
        private readonly IContractCreationTransactionProcessor _contractCreationTransactionProcessor;
        private readonly List<ITransactionFilter> _transactionFilters;
        private readonly List<ITransactionReceiptFilter> _transactionReceiptFilters;

        protected readonly ITransactionProxy TransactionProxy;

        public bool EnabledContractCreationProcessing { get; set; } = true;
        public bool EnabledContractProcessing { get; set; } = true;
        public bool EnabledValueProcessing { get; set; } = true;
        public IContractTransactionProcessor ContractTransactionProcessor { get; }

        public TransactionProcessor(
            ITransactionProxy transactionProxy, 
            IContractTransactionProcessor contractTransactionProcessor, 
            IValueTransactionProcessor valueTransactionProcessor, 
            IContractCreationTransactionProcessor contractCreationTransactionProcessor,
            IEnumerable<ITransactionFilter> transactionFilters = null,
            IEnumerable<ITransactionReceiptFilter> transactionReceiptFilters = null)
        {
            TransactionProxy = transactionProxy;
            ContractTransactionProcessor = contractTransactionProcessor;
            _valueTransactionProcessor = valueTransactionProcessor;
            _contractCreationTransactionProcessor = contractCreationTransactionProcessor;

            _transactionFilters = new List<ITransactionFilter>(
                transactionFilters ?? new ITransactionFilter[0]);

            _transactionReceiptFilters = new List<ITransactionReceiptFilter>(
                transactionReceiptFilters ?? new ITransactionReceiptFilter[0]);
        }
       
        public virtual async Task ProcessTransactionAsync(string transactionHash, BlockWithTransactionHashes block)
        {
            var transactionSource = await TransactionProxy.GetTransactionByHash(transactionHash)
                .ConfigureAwait(false);

            if (await _transactionFilters.IgnoreAsync(transactionSource)) return;

            var transactionReceipt = await TransactionProxy.GetTransactionReceipt(transactionHash)
                .ConfigureAwait(false);

            if (await _transactionReceiptFilters.IgnoreAsync(transactionReceipt)) return;
            
            if ( transactionSource.IsForContractCreation(transactionReceipt))
            {
                if (EnabledContractCreationProcessing)
                {
                    await
                        _contractCreationTransactionProcessor.ProcessTransactionAsync(
                            transactionSource, transactionReceipt, block.Timestamp)
                            .ConfigureAwait(false);
                }
            }
            else
            {
                if (await ContractTransactionProcessor.IsTransactionForContractAsync(transactionSource))
                {
                    if (EnabledContractProcessing)
                    {
                        await
                            ContractTransactionProcessor.ProcessTransactionAsync(
                                transactionSource, transactionReceipt, block.Timestamp)
                                .ConfigureAwait(false);
                    }
                }
                else if (EnabledValueProcessing)
                {
                    await
                        _valueTransactionProcessor.ProcessTransactionAsync(
                            transactionSource, transactionReceipt, block.Timestamp)
                            .ConfigureAwait(false);
                }
            }
        }

    }
}