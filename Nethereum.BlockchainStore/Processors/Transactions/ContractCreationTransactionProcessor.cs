using Nethereum.BlockchainStore.Handlers;
using Nethereum.BlockchainStore.Web3Abstractions;
using Nethereum.Hex.HexTypes;
using Nethereum.RPC.Eth.DTOs;
using System.Threading.Tasks;
using Transaction = Nethereum.RPC.Eth.DTOs.Transaction;

namespace Nethereum.BlockchainStore.Processors.Transactions
{
    public class ContractCreationTransactionProcessor : IContractCreationTransactionProcessor
    {
        private readonly IGetCode _getCodeProxy;
        private readonly IContractHandler _contractHandler;
        private readonly ITransactionHandler _transactionHandler;

        public ContractCreationTransactionProcessor(
          IGetCode getCodeProxy, 
          IContractHandler contractHandler, 
          ITransactionHandler transactionHandler)
        {
            _getCodeProxy = getCodeProxy;
            _contractHandler = contractHandler;
            _transactionHandler = transactionHandler;
        }

        public virtual async Task ProcessTransactionAsync(
            Transaction transaction, 
            TransactionReceipt transactionReceipt, 
            HexBigInteger blockTimestamp)
        {
            if (!transaction.IsForContractCreation(transactionReceipt)) return;

            var contractAddress = transactionReceipt.ContractAddress;
            var code = await _getCodeProxy.GetCode(contractAddress).ConfigureAwait(false);
            var failedCreatingContract = HasFailedToCreateContract(code);

            if (!failedCreatingContract)
                await _contractHandler.HandleAsync(new ContractTransaction(contractAddress, code, transaction))
                    .ConfigureAwait(false);

            await _transactionHandler.HandleContractCreationTransactionAsync(new ContractCreationTransaction(contractAddress, code, transaction, transactionReceipt, failedCreatingContract, blockTimestamp));
        }

        protected virtual bool HasFailedToCreateContract(string code)
        {
            return (code == null) || (code == "0x");
        }


    }
}