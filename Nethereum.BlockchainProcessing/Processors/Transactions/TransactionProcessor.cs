using System.Collections.Generic;
using System.Threading.Tasks;
using Nethereum.BlockchainProcessing.Web3Abstractions;
using Nethereum.RPC.Eth.DTOs;

namespace Nethereum.BlockchainProcessing.Processors.Transactions
{
    public class TransactionProcessor : ITransactionProcessor
    {
        private readonly IValueTransactionProcessor _valueTransactionProcessor;
        private readonly IContractCreationTransactionProcessor _contractCreationTransactionProcessor;
        private readonly ITransactionLogProcessor _transactionLogProcessor;
        private readonly List<ITransactionFilter> _transactionFilters;
        private readonly List<ITransactionReceiptFilter> _transactionReceiptFilters;
        private readonly List<ITransactionAndReceiptFilter> _transactionAndReceiptFilters;

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
            ITransactionLogProcessor transactionLogProcessor,
            IEnumerable<ITransactionFilter> transactionFilters = null,
            IEnumerable<ITransactionReceiptFilter> transactionReceiptFilters = null,
            IEnumerable<ITransactionAndReceiptFilter> transactionAndReceiptFilters = null)
        {
            TransactionProxy = transactionProxy;
            ContractTransactionProcessor = contractTransactionProcessor;
            _valueTransactionProcessor = valueTransactionProcessor;
            _contractCreationTransactionProcessor = contractCreationTransactionProcessor;
            _transactionLogProcessor = transactionLogProcessor;
            _transactionFilters = new List<ITransactionFilter>(
                transactionFilters ?? new ITransactionFilter[0]);

            _transactionReceiptFilters = new List<ITransactionReceiptFilter>(
                transactionReceiptFilters ?? new ITransactionReceiptFilter[0]);

            _transactionAndReceiptFilters = new List<ITransactionAndReceiptFilter>(
                transactionAndReceiptFilters ?? new ITransactionAndReceiptFilter[0]);
        }
       
        public virtual async Task ProcessTransactionAsync(Block block, Transaction transactionSource)
        {
            if (await _transactionFilters.IgnoreAsync(transactionSource)) return;

            var transactionReceipt = await TransactionProxy
                .GetTransactionReceipt(transactionSource.TransactionHash)
                .ConfigureAwait(false);

            if (await _transactionReceiptFilters.IgnoreAsync(transactionReceipt)) return;

            if (await _transactionAndReceiptFilters.IgnoreAsync((transactionSource, transactionReceipt))) return;

            await _transactionLogProcessor.ProcessAsync(transactionSource, transactionReceipt);

            if (transactionSource.IsForContractCreation(transactionReceipt))
            {
                await ProcessContractCreation(block, transactionSource, transactionReceipt);
                return;
            }

            if (await ContractTransactionProcessor.IsTransactionForContractAsync(transactionSource))
            {
                await ProcessContractTransaction(block, transactionSource, transactionReceipt);
                return;
            }

            await ProcessValueTransaction(block, transactionSource, transactionReceipt);
        }

        private async Task ProcessValueTransaction(Block block, Transaction transactionSource, TransactionReceipt transactionReceipt)
        {
            if (!EnabledValueProcessing) return;

            await
                _valueTransactionProcessor.ProcessTransactionAsync(
                        transactionSource, transactionReceipt, block.Timestamp)
                    .ConfigureAwait(false);
        }

        private async Task ProcessContractTransaction(Block block, Transaction transactionSource, TransactionReceipt transactionReceipt)
        {
            if (!EnabledContractProcessing) return;

            await
                ContractTransactionProcessor.ProcessTransactionAsync(
                        transactionSource, transactionReceipt, block.Timestamp)
                    .ConfigureAwait(false);
        }

        private async Task ProcessContractCreation(Block block, Transaction transactionSource, TransactionReceipt transactionReceipt)
        {
            if (!EnabledContractCreationProcessing) return;

            await
                _contractCreationTransactionProcessor.ProcessTransactionAsync(
                        transactionSource, transactionReceipt, block.Timestamp)
                    .ConfigureAwait(false);
        }
    }
}