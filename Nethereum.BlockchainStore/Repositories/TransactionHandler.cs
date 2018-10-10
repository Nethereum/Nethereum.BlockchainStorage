using Nethereum.BlockchainStore.Handlers;
using System.Threading.Tasks;

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

        public async Task HandleContractCreationTransactionAsync(ContractCreationTransaction contractCreationTransaction)
        {
            await _transactionRepository.UpsertAsync(
                contractCreationTransaction.ContractAddress, contractCreationTransaction.Code, contractCreationTransaction.Transaction, contractCreationTransaction.TransactionReceipt, contractCreationTransaction.FailedCreatingContract, contractCreationTransaction.BlockTimestamp);

            if (_addressTransactionRepository != null)
            {
                await _addressTransactionRepository.UpsertAsync(
                    contractCreationTransaction.Transaction,
                    contractCreationTransaction.TransactionReceipt,
                    contractCreationTransaction.FailedCreatingContract, contractCreationTransaction.BlockTimestamp, 
                    contractCreationTransaction.Transaction.From, null, false, contractCreationTransaction.ContractAddress);
            }
        }

        public async Task HandleAddressTransactionAsync(AddressTransactionWithReceipt addressTransactionWithReceipt)
        {
            if (_addressTransactionRepository != null)
            {
                await
                    _addressTransactionRepository.UpsertAsync(
                        addressTransactionWithReceipt.Transaction, addressTransactionWithReceipt.TransactionReceipt, addressTransactionWithReceipt.HasError, addressTransactionWithReceipt.BlockTimestamp,
                        addressTransactionWithReceipt.Address, addressTransactionWithReceipt.Error, addressTransactionWithReceipt.HasVmStack);
            }
        }

        public async Task HandleTransactionAsync(TransactionWithReceipt transactionWithReceipt)
        {
            await
                _transactionRepository.UpsertAsync(
                    transactionWithReceipt.Transaction, transactionWithReceipt.TransactionReceipt, transactionWithReceipt.HasError, transactionWithReceipt.BlockTimestamp, transactionWithReceipt.HasVmStack, transactionWithReceipt.Error);

            if (_addressTransactionRepository != null)
            {
                await
                    _addressTransactionRepository.UpsertAsync(
                        transactionWithReceipt.Transaction, transactionWithReceipt.TransactionReceipt, transactionWithReceipt.HasError, transactionWithReceipt.BlockTimestamp,
                        transactionWithReceipt.Transaction.To, transactionWithReceipt.Error, transactionWithReceipt.HasVmStack);
            }
        }
    }
}
