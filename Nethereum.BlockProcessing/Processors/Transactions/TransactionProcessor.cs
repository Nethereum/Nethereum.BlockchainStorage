using Nethereum.RPC.Eth.DTOs;
using Nethereum.RPC.Eth.Services;
using Nethereum.Web3;
using System.Collections.Generic;
using System.Threading.Tasks;

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

        protected readonly IEthApiTransactionsService TransactionProxy;

        public bool EnabledContractCreationProcessing { get; set; } = true;
        public bool EnabledContractProcessing { get; set; } = true;
        public bool EnabledValueProcessing { get; set; } = true;
        public IContractTransactionProcessor ContractTransactionProcessor { get; }

        public TransactionProcessor(
            IWeb3 web3, 
            IContractTransactionProcessor contractTransactionProcessor, 
            IValueTransactionProcessor valueTransactionProcessor, 
            IContractCreationTransactionProcessor contractCreationTransactionProcessor,
            ITransactionLogProcessor transactionLogProcessor,
            IEnumerable<ITransactionFilter> transactionFilters = null,
            IEnumerable<ITransactionReceiptFilter> transactionReceiptFilters = null,
            IEnumerable<ITransactionAndReceiptFilter> transactionAndReceiptFilters = null)
        {
            TransactionProxy = web3.Eth.Transactions;
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
       
        public virtual async Task ProcessTransactionAsync(Block block, Transaction tx)
        {
            if (await _transactionFilters.IgnoreAsync(tx)) return;

            var receipt = await TransactionProxy
                .GetTransactionReceipt.SendRequestAsync(tx.TransactionHash)
                .ConfigureAwait(false);

            if (await _transactionReceiptFilters
                .IgnoreAsync(receipt).ConfigureAwait(false)) return;

            if (await _transactionAndReceiptFilters.IgnoreAsync((tx, receipt))
                .ConfigureAwait(false)) return;

            await _transactionLogProcessor.ProcessAsync(tx, receipt)
                .ConfigureAwait(false);

            if (tx.IsForContractCreation(receipt))
            {
                await ProcessContractCreation(block, tx, receipt)
                    .ConfigureAwait(false);
                return;
            }

            if (await ContractTransactionProcessor.IsTransactionForContractAsync(tx))
            {
                await ProcessContractTransaction(block, tx, receipt)
                    .ConfigureAwait(false);
                return;
            }

            await ProcessValueTransaction(block, tx, receipt)
                .ConfigureAwait(false);
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