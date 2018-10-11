using Nethereum.BlockchainStore.Handlers;
using Nethereum.BlockchainStore.Web3Abstractions;
using Nethereum.Hex.HexTypes;
using Nethereum.RPC.Eth.DTOs;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Transaction = Nethereum.RPC.Eth.DTOs.Transaction;

namespace Nethereum.BlockchainStore.Processors.Transactions
{
    public class ContractTransactionProcessor : IContractTransactionProcessor
    {
        private readonly IGetTransactionVMStack _vmStackProxy;
        private readonly IVmStackErrorChecker _vmStackErrorChecker;
        private readonly IContractHandler _contractHandler;
        private readonly ITransactionHandler _transactionHandler;
        private readonly ITransactionVMStackHandler _transactionVmStackHandler;

        public ContractTransactionProcessor(
          IGetTransactionVMStack vmStackProxy, 
          IVmStackErrorChecker vmStackErrorChecker,
          IContractHandler contractHandler,
          ITransactionHandler transactionHandler, 
          ITransactionVMStackHandler transactionVmStackHandler)
        {
            _vmStackProxy = vmStackProxy;
            _vmStackErrorChecker = vmStackErrorChecker;
            _contractHandler = contractHandler;
            _transactionHandler = transactionHandler;
            _transactionVmStackHandler = transactionVmStackHandler;
        }

        public async Task<bool> IsTransactionForContractAsync(Transaction transaction)
        {
            if (transaction.To.IsAnEmptyAddress()) return false;
            return await _contractHandler.ExistsAsync(transaction.To)
                .ConfigureAwait(false);
        }

        public bool EnabledVmProcessing { get; set; } = true;

        public async Task ProcessTransactionAsync(
            Transaction transaction, 
            TransactionReceipt transactionReceipt, 
            HexBigInteger blockTimestamp)
        {
            var transactionHash = transaction.TransactionHash;
            var hasStackTrace = false;
            JObject stackTrace = null;
            var error = string.Empty;
            var hasError = transactionReceipt.Failed();

            if (EnabledVmProcessing)
            {
                try
                {
                    stackTrace = await _vmStackProxy
                        .GetTransactionVmStack(transactionHash)
                        .ConfigureAwait(false);
                }
                catch
                {

                }

                if (stackTrace != null)
                {
                    error = _vmStackErrorChecker.GetError(stackTrace);
                    hasError = !string.IsNullOrEmpty(error);
                    hasStackTrace = true;

                    await _transactionVmStackHandler.HandleAsync
                        (new TransactionVmStack(transactionHash, transaction.To, stackTrace));
                }
            }

            await _transactionHandler.HandleTransactionAsync(
                new TransactionWithReceipt(
                    transaction, 
                    transactionReceipt, 
                    hasError, 
                    blockTimestamp, 
                    error, 
                    hasStackTrace));

        }
    }
}