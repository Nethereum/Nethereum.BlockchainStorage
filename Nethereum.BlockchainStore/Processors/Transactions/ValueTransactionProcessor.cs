using System.Threading.Tasks;
using Nethereum.BlockchainStore.Repositories;
using Nethereum.Hex.HexTypes;
using Nethereum.RPC.Eth.DTOs;
using Transaction = Nethereum.RPC.Eth.DTOs.Transaction;

namespace Nethereum.BlockchainStore.Processors.Transactions
{
    public class ValueTransactionProcessor : IValueTransactionProcessor
    {
        private readonly ITransactionRepository _transactionRepository;
        private readonly IAddressTransactionRepository _addressTransactionRepository;

        public ValueTransactionProcessor(ITransactionRepository transactionRepository, IAddressTransactionRepository addressTransactionRepository)
        {
            _transactionRepository = transactionRepository;
            _addressTransactionRepository = addressTransactionRepository;
        }

        public async Task ProcessTransactionAsync(Transaction transaction, TransactionReceipt transactionReceipt, HexBigInteger blockTimestamp)
        {
            await _transactionRepository.UpsertAsync(transaction,
                transactionReceipt,
                false, blockTimestamp).ConfigureAwait(false);
            await _addressTransactionRepository.UpsertAsync(transaction,
                transactionReceipt,
                false, blockTimestamp, transaction.To).ConfigureAwait(false);
        }
    }
}