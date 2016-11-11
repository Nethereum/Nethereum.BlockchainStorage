using System.Threading.Tasks;
using Nethereum.RPC.Eth.DTOs;

namespace Nethereum.BlockchainStore.Processors.Transactions
{
    public class TransactionProcessor : ITransactionProcessor
    {
        private readonly IContractTransactionProcessor _contractTransactionProcessor;
        private readonly IValueTransactionProcessor _valueTransactionProcessor;
        private readonly IContractCreationTransactionProcessor _contractCreationTransactionProcessor;

        public TransactionProcessor(Web3.Web3 web3, IContractTransactionProcessor contractTransactionProcessor, IValueTransactionProcessor valueTransactionProcessor, IContractCreationTransactionProcessor contractCreationTransactionProcessor)
        {
            _contractTransactionProcessor = contractTransactionProcessor;
            _valueTransactionProcessor = valueTransactionProcessor;
            _contractCreationTransactionProcessor = contractCreationTransactionProcessor;
            Web3 = web3;
        }

        protected Web3.Web3 Web3 { get; }
       
        public virtual async Task ProcessTransactionAsync(string transactionHash, BlockWithTransactionHashes block)
        {
            var transactionSource = await GetTransaction(transactionHash).ConfigureAwait(false);
            var transactionReceipt = await GetTransactionReceipt(transactionHash).ConfigureAwait(false);

            if (_contractCreationTransactionProcessor.IsTransactionForContractCreation(transactionSource, transactionReceipt))
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

        public bool EnabledContractCreationProcessing { get; set; } = true;
        public bool EnabledContractProcessing { get; set; } = true;
        public bool EnabledValueProcessing { get; set; } = true;
        public IContractTransactionProcessor ContractTransactionProcessor => _contractTransactionProcessor;

        public async Task<Transaction> GetTransaction(string txnHash)
        {
            return await Web3.Eth.Transactions.GetTransactionByHash.SendRequestAsync(txnHash).ConfigureAwait(false);
        }

        public async Task<TransactionReceipt> GetTransactionReceipt(string txnHash)
        {
            return await Web3.Eth.Transactions.GetTransactionReceipt.SendRequestAsync(txnHash).ConfigureAwait(false);
        }
    }
}