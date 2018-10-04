using System.Threading.Tasks;
using Nethereum.BlockchainStore.Handlers;
using Nethereum.Hex.HexTypes;
using Nethereum.RPC.Eth.DTOs;

namespace Nethereum.BlockchainStore.Repositories
{
    public class TransactionHandler : ITransactionHandler
    {
        private readonly ITransactionRepository _transactionRepository;
        private readonly IAddressTransactionRepository _addressTransactionRepository;

        public TransactionHandler(
            ITransactionRepository transactionRepository, 
            IAddressTransactionRepository addressTransactionRepository = null)
        {
            this._transactionRepository = transactionRepository;
            this._addressTransactionRepository = addressTransactionRepository;
        }

        public async Task HandleContractCreationTransactionAsync(string contractAddress, string code, Transaction transaction, TransactionReceipt transactionReceipt, 
            bool failedCreatingContract, HexBigInteger blockTimestamp)
        {
            await _transactionRepository.UpsertAsync(
                contractAddress, code, transaction, transactionReceipt, failedCreatingContract, blockTimestamp);

            if (_addressTransactionRepository != null)
            {
                await _addressTransactionRepository.UpsertAsync(
                    transaction,
                    transactionReceipt,
                    failedCreatingContract, blockTimestamp, null, null, false, contractAddress);
            }
        }

        public async Task HandleAddressTransactionAsync(
            Transaction transaction, TransactionReceipt transactionReceipt, bool hasError, HexBigInteger blockTimestamp, 
            string address, string error = null, bool hasVmStack = false)
        {
            if (_addressTransactionRepository != null)
            {
                await
                    _addressTransactionRepository.UpsertAsync(
                        transaction, transactionReceipt, hasError, blockTimestamp,
                        address, error, hasVmStack);
            }
        }

        public async Task HandleTransactionAsync(
            Transaction transaction, TransactionReceipt transactionReceipt, bool hasError, 
            HexBigInteger blockTimestamp, string error = null, bool hasVmStack = false)
        {
            await
                _transactionRepository.UpsertAsync(
                    transaction, transactionReceipt, hasError, blockTimestamp, hasVmStack, error);

            if (_addressTransactionRepository != null)
            {
                await
                    _addressTransactionRepository.UpsertAsync(
                        transaction, transactionReceipt, hasError, blockTimestamp,
                        transaction.To, error, hasVmStack);
            }
        }
    }
}
