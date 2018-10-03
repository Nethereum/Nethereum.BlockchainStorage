using Nethereum.Hex.HexTypes;
using Nethereum.RPC.Eth.DTOs;
using System.Threading.Tasks;

namespace Nethereum.BlockchainStore.Handlers
{
    public interface ITransactionHandler
    {
        Task HandleContractCreationTransactionAsync(
            string contractAddress, string code, Transaction transaction, 
            TransactionReceipt transactionReceipt, bool failedCreatingContract, 
            HexBigInteger blockTimestamp);

        Task HandleTransactionAsync(
            Transaction transaction, TransactionReceipt transactionReceipt, bool hasError, 
            HexBigInteger blockTimestamp, string error = null, 
            bool hasVmStack = false);

        Task HandleAddressTransactionAsync(
            Transaction transaction, TransactionReceipt transactionReceipt, bool hasError, 
            HexBigInteger blockTimestamp, string address, string error = null, 
            bool hasVmStack = false);

    }
}
