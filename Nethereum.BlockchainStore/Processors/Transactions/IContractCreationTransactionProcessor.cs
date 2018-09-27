using System.Threading.Tasks;
using Nethereum.Hex.HexTypes;
using Nethereum.RPC.Eth.DTOs;

namespace Nethereum.BlockchainStore.Processors.Transactions
{
    public interface IContractCreationTransactionProcessor
    {
        Task ProcessTransactionAsync(Transaction transaction, TransactionReceipt transactionReceipt, HexBigInteger blockTimestamp);
    }
}