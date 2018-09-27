using Nethereum.BlockchainStore.Repositories;
using Nethereum.Hex.HexTypes;
using Nethereum.RPC.Eth.DTOs;
using System.Threading.Tasks;
using Nethereum.BlockchainStore.Web3Abstractions;
using Transaction = Nethereum.RPC.Eth.DTOs.Transaction;

namespace Nethereum.BlockchainStore.Processors.Transactions
{
    public class ContractCreationTransactionProcessor : IContractCreationTransactionProcessor
    {
        private readonly IGetCode _getCodeProxy;
        private readonly IContractRepository _contractRepository;
        private readonly ITransactionRepository _transactionRepository;
        private readonly IAddressTransactionRepository _addressTransactionRepository;


        public ContractCreationTransactionProcessor(
          IGetCode getCodeProxy, IContractRepository contractRepository, ITransactionRepository transactionRepository, IAddressTransactionRepository addressTransactionRepository)
        {
            _getCodeProxy = getCodeProxy;
            _contractRepository = contractRepository;
            _transactionRepository = transactionRepository;
            _addressTransactionRepository = addressTransactionRepository;
        }

        public virtual async Task ProcessTransactionAsync(Transaction transaction, TransactionReceipt transactionReceipt, HexBigInteger blockTimestamp)
        {
            if (!transaction.IsForContractCreation(transactionReceipt)) return;

            var contractAddress = transactionReceipt.ContractAddress;
            var code = await _getCodeProxy.GetCode(contractAddress).ConfigureAwait(false);
            var failedCreatingContract = HasFailedToCreateContract(code);

            if (!failedCreatingContract)
                await _contractRepository.UpsertAsync(contractAddress, code, transaction).ConfigureAwait(false);

            await _transactionRepository.UpsertAsync(contractAddress, code,
                transaction, transactionReceipt,
                failedCreatingContract, blockTimestamp);

            await _addressTransactionRepository.UpsertAsync(
                transaction,
                transactionReceipt,
                failedCreatingContract, blockTimestamp, null, null, false, contractAddress);
        }

        protected virtual bool HasFailedToCreateContract(string code)
        {
            return (code == null) || (code == "0x");
        }


    }
}