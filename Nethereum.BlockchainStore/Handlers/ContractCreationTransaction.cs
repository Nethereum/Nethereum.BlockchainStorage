using Nethereum.Hex.HexTypes;
using Nethereum.RPC.Eth.DTOs;

namespace Nethereum.BlockchainStore.Handlers
{
    public class ContractCreationTransaction
    {
        public ContractCreationTransaction(string contractAddress, string code, Transaction transaction, TransactionReceipt transactionReceipt, bool failedCreatingContract, HexBigInteger blockTimestamp)
        {
            ContractAddress = contractAddress;
            Code = code;
            Transaction = transaction;
            TransactionReceipt = transactionReceipt;
            FailedCreatingContract = failedCreatingContract;
            BlockTimestamp = blockTimestamp;
        }

        public string ContractAddress { get; private set; }
        public string Code { get; private set; }
        public Transaction Transaction { get; private set; }
        public TransactionReceipt TransactionReceipt { get; private set; }
        public bool FailedCreatingContract { get; private set; }
        public HexBigInteger BlockTimestamp { get; private set; }
    }
}
