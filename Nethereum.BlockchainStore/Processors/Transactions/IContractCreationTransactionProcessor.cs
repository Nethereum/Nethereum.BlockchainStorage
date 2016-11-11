using System.Threading.Tasks;
using Nethereum.Hex.HexTypes;
using Nethereum.RPC.Eth.DTOs;

namespace Nethereum.BlockchainStore.Processors.Transactions
{
    public interface IContractCreationTransactionProcessor
    {
        bool IsTransactionForContractCreation(Transaction transaction, TransactionReceipt transactionReceipt);
        Task ProcessTransactionAsync(Transaction transaction, TransactionReceipt transactionReceipt, HexBigInteger blockTimestamp);
    }
}