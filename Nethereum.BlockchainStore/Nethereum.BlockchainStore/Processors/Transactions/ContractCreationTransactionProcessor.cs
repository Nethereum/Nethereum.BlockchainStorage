using System.Threading.Tasks;
using Nethereum.BlockchainStore.Repositories;
using Nethereum.Hex.HexTypes;
using Nethereum.RPC.Eth.DTOs;
using Transaction = Nethereum.RPC.Eth.DTOs.Transaction;

namespace Nethereum.BlockchainStore.Processors.Transactions
{
    public class ContractCreationTransactionProcessor : IContractCreationTransactionProcessor
    {
        private readonly Web3.Web3 _web3;
        private readonly IContractRepository _contractRepository;
        private readonly ITransactionRepository _transactionRepository;
        private readonly IAddressTransactionRepository _addressTransactionRepository;


        public ContractCreationTransactionProcessor(
          Web3.Web3 web3, IContractRepository contractRepository, ITransactionRepository transactionRepository, IAddressTransactionRepository addressTransactionRepository)
        {
            _web3 = web3;
            _contractRepository = contractRepository;
            _transactionRepository = transactionRepository;
            _addressTransactionRepository = addressTransactionRepository;
        }

        public async Task ProcessTransactionAsync(Transaction transaction, TransactionReceipt transactionReceipt, HexBigInteger blockTimestamp)
        {
            if (!IsTransactionForContractCreation(transaction, transactionReceipt)) return;

            var contractAddress = GetContractAddress(transactionReceipt);
            var code = await GetCode(contractAddress).ConfigureAwait(false);
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

        public async Task<string> GetCode(string contractAddres)
        {
            return await _web3.Eth.GetCode.SendRequestAsync(contractAddres).ConfigureAwait(false);
        }

        public bool HasFailedToCreateContract(string code)
        {
            return (code == null) || (code == "0x");
        }

        public bool IsTransactionForContractCreation(Transaction transaction, TransactionReceipt transactionReceipt)
        {
            return string.IsNullOrEmpty(transaction.To) && !string.IsNullOrEmpty(GetContractAddress(transactionReceipt));
        }

        private string GetContractAddress(TransactionReceipt transactionReceipt)
        {
            return transactionReceipt.ContractAddress;
        }
    }
}