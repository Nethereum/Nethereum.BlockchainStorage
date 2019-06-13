using System.Threading.Tasks;
using Nethereum.BlockchainProcessing.Handlers;
using Nethereum.Hex.HexTypes;
using Nethereum.RPC.Eth;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.Web3;
using Transaction = Nethereum.RPC.Eth.DTOs.Transaction;

namespace Nethereum.BlockchainProcessing.Processors.Transactions
{
    public class ContractCreationTransactionProcessor : IContractCreationTransactionProcessor
    {
        private readonly IEthGetCode _getCodeProxy;
        private readonly IContractHandler _contractHandler;
        private readonly ITransactionHandler _transactionHandler;

        public ContractCreationTransactionProcessor(
          IWeb3 web3, 
          IContractHandler contractHandler, 
          ITransactionHandler transactionHandler)
        {
            _getCodeProxy = web3.Eth.GetCode;
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
            var code = await _getCodeProxy.SendRequestAsync(contractAddress).ConfigureAwait(false);
            var failedCreatingContract = HasFailedToCreateContract(code);

            if (!failedCreatingContract)
                await _contractHandler.HandleAsync(new ContractTransaction(contractAddress, code, transaction))
                    .ConfigureAwait(false);

            await _transactionHandler
                .HandleContractCreationTransactionAsync(
                    new ContractCreationTransaction(
                        contractAddress, 
                        code, 
                        transaction, 
                        transactionReceipt, 
                        failedCreatingContract, 
                        blockTimestamp))
                .ConfigureAwait(false);
        }

        protected virtual bool HasFailedToCreateContract(string code)
        {
            return (code == null) || (code == "0x");
        }


    }
}