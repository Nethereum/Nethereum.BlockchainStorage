using Nethereum.RPC.Eth.DTOs;

namespace Nethereum.BlockchainProcessing.Handlers
{
    public class ContractTransaction
    {
        public ContractTransaction(string contractAddress, string code, Transaction transaction)
        {
            ContractAddress = contractAddress;
            Code = code;
            Transaction = transaction;
        }

        public string ContractAddress { get; private set; }
        public string Code { get; private set; }
        public Transaction Transaction { get; private set; }
    }
}
