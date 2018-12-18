using System;
using System.IO;
using System.Threading.Tasks;
using Nethereum.BlockchainProcessing.Handlers;
using Nethereum.BlockchainProcessing.Web3Abstractions;
using Nethereum.Configuration;
using Nethereum.Hex.HexTypes;
using Nethereum.RPC.Eth.DTOs;
using Newtonsoft.Json.Linq;
using Transaction = Nethereum.RPC.Eth.DTOs.Transaction;

namespace Nethereum.BlockchainProcessing.Processors.Transactions
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
                catch(Exception x)
                {
                    
                }

                if (stackTrace != null)
                {
                    //TODO!  _Remove this debug line
                    //File.WriteAllText($"c:/Temp/StackTrace_{transactionReceipt.BlockNumber.Value}.json", stackTrace.ToString());

                    error = _vmStackErrorChecker.GetError(stackTrace);
                    hasError = !string.IsNullOrEmpty(error);
                    hasStackTrace = true;

                    await _transactionVmStackHandler.HandleAsync
                        (new TransactionVmStack(transactionHash, transaction.To, stackTrace))
                        .ConfigureAwait(false);
                }
            }

            var tx = new TransactionWithReceipt(
                transaction,
                transactionReceipt,
                hasError,
                blockTimestamp,
                error,
                hasStackTrace);

            await _transactionHandler.HandleTransactionAsync(tx)
                .ConfigureAwait(false);

        }
    }
}