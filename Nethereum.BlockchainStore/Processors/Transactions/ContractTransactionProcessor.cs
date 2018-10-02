using Nethereum.BlockchainStore.Repositories;
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
        private readonly IContractRepository _contractRepository;
        private readonly ITransactionRepository _transactionRepository;
        private readonly IAddressTransactionRepository _addressTransactionRepository;
        private readonly ITransactionVMStackRepository _transactionVmStackRepository;
        private readonly ITransactionLogRepository _transactionLogRepository;

        private readonly IEnumerable<ITransactionLogFilter> _transactionLogFilters;

        public ContractTransactionProcessor(
          IGetTransactionVMStack vmStackProxy, 
          IVmStackErrorChecker vmStackErrorChecker,
          IContractRepository contractRepository, 
          ITransactionRepository transactionRepository, 
          IAddressTransactionRepository addressTransactionRepository,
          ITransactionVMStackRepository transactionVmStackRepository,
          ITransactionLogRepository transactionLogRepository,
          IEnumerable<ITransactionLogFilter> transactionLogFilters = null)
        {
            _vmStackProxy = vmStackProxy;
            _vmStackErrorChecker = vmStackErrorChecker;
            _contractRepository = contractRepository;
            _transactionRepository = transactionRepository;
            _addressTransactionRepository = addressTransactionRepository;
            _transactionVmStackRepository = transactionVmStackRepository;
            _transactionLogRepository = transactionLogRepository;
            _transactionLogFilters = transactionLogFilters;
        }

        public async Task<bool> IsTransactionForContractAsync(Transaction transaction)
        {
            if (transaction.To.IsAnEmptyAddress()) return false;
            return await _contractRepository.ExistsAsync(transaction.To)
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
            var hasError = false;

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
                    if (transaction.Gas == transactionReceipt.GasUsed)
                        hasError = true;
                }

                if (stackTrace != null)
                {
                    error = _vmStackErrorChecker.GetError(stackTrace);
                    hasError = !string.IsNullOrEmpty(error);
                    hasStackTrace = true;
                    await _transactionVmStackRepository.UpsertAsync
                        (transactionHash, transaction.To, stackTrace);
                }
            }


            await _transactionRepository.UpsertAsync(
                transaction, transactionReceipt,
                hasError, blockTimestamp, hasStackTrace, error);

            await
                _addressTransactionRepository.UpsertAsync(
                    transaction, transactionReceipt, hasError, blockTimestamp,
                    transaction.To, error, hasStackTrace);

            if (transactionReceipt.Logs == null) return;

            var logs = transactionReceipt.Logs;

            var addressesAdded = new HashSet<string>(StringComparer.InvariantCultureIgnoreCase)
                {transaction.To};

            for (var i = 0; i < logs.Count; i++)
            {
                if (logs[i] is JObject log)
                { 
                    var logAddress = log["address"].Value<string>();

                    if (!addressesAdded.Contains(logAddress))
                    {
                        addressesAdded.Add(logAddress);

                        await
                            _addressTransactionRepository.UpsertAsync(
                                transaction, transactionReceipt, hasError,
                                blockTimestamp, logAddress, error, hasStackTrace);
                    }

                    if (await _transactionLogFilters.IsMatchAsync(log))
                    {
                        await _transactionLogRepository.UpsertAsync(transactionHash, i, log);
                    }

                }
            }
        }
    }
}