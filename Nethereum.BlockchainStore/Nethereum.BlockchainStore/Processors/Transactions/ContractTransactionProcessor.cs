using System.Collections.Generic;
using System.Threading.Tasks;
using Nethereum.BlockchainStore.Repositories;
using Nethereum.Hex.HexTypes;
using Nethereum.RPC.DebugGeth.DTOs;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.Web3;
using Newtonsoft.Json.Linq;
using Transaction = Nethereum.RPC.Eth.DTOs.Transaction;

namespace Nethereum.BlockchainStore.Processors.Transactions
{
    public class ContractTransactionProcessor : IContractTransactionProcessor
    {
        private readonly IContractRepository _contractRepository;
        private readonly ITransactionRepository _transactionRepository;
        private readonly IAddressTransactionRepository _addressTransactionRepository;
        private readonly ITransactionVMStackRepository _transactionVmStackRepository;
        private readonly ITransactionLogRepository _transactionLogRepository;
        protected VmStackErrorChecker VmStackErrorChecker { get; set; }
        protected Web3.Web3 Web3 { get; set; }

        public ContractTransactionProcessor(
          Web3.Web3 web3, IContractRepository contractRepository, 
                          ITransactionRepository transactionRepository, 
                          IAddressTransactionRepository addressTransactionRepository,
                          ITransactionVMStackRepository transactionVmStackRepository,
                          ITransactionLogRepository transactionLogRepository)
        {
            Web3 = web3;
            _contractRepository = contractRepository;
            _transactionRepository = transactionRepository;
            _addressTransactionRepository = addressTransactionRepository;
            _transactionVmStackRepository = transactionVmStackRepository;
            _transactionLogRepository = transactionLogRepository;
            VmStackErrorChecker = new VmStackErrorChecker();
        }

        public async Task<bool> IsTransactionForContractAsync(Transaction transaction)
        {
            if (transaction.To == null) return false;
            return await _contractRepository.ExistsAsync(transaction.To).ConfigureAwait(false);
        }

        public bool EnabledVmProcessing { get; set; } = true;

        public async Task ProcessTransactionAsync(Transaction transaction, TransactionReceipt transactionReceipt, HexBigInteger blockTimestamp)
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
                    stackTrace = await GetTransactionVmStack(transactionHash).ConfigureAwait(false);
                }
                catch
                {
                    if (transaction.Gas == transactionReceipt.GasUsed)
                        hasError = true;
                }

                if (stackTrace != null)
                {
                    error = VmStackErrorChecker.GetError(stackTrace);
                    hasError = !string.IsNullOrEmpty(error);
                    hasStackTrace = true;
                    await _transactionVmStackRepository.UpsertAsync(transactionHash, transaction.To, stackTrace);
                }
            }

            var logs = transactionReceipt.Logs;

            await _transactionRepository.UpsertAsync(transaction,
                transactionReceipt,
                hasError, blockTimestamp, hasStackTrace, error);

            await
                _addressTransactionRepository.UpsertAsync(transaction, transactionReceipt, hasError, blockTimestamp,
                    transaction.To, error, hasStackTrace);

            var addressesAdded = new List<string> {transaction.To};

            for (var i = 0; i < logs.Count; i++)
            {
                var log = logs[i] as JObject;
                if (log != null)
                {
                    var logAddress = log["address"].Value<string>();

                    if (!addressesAdded.Exists(x => x == logAddress))
                    {
                        addressesAdded.Add(logAddress);

                        await
                            _addressTransactionRepository.UpsertAsync(transaction, transactionReceipt, hasError,
                                blockTimestamp,
                                logAddress, error, hasStackTrace);
                    }

                    await _transactionLogRepository.UpsertAsync(transactionHash,
                        i, log);
                    
                }
            }
        }

        protected async Task<JObject> GetTransactionVmStack(string transactionHash)
        {
            return
                await
                    Web3.DebugGeth.TraceTransaction.SendRequestAsync(transactionHash,
                        new TraceTransactionOptions {DisableMemory = true, DisableStorage = true, DisableStack = true});
        }
    }
}