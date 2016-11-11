using System.Threading.Tasks;
using Nethereum.Hex.HexTypes;
using Nethereum.RPC.Eth.DTOs;

namespace Nethereum.BlockchainStore.Repositories
{
    public interface ITransactionRepository
    {
        Task UpsertAsync(string contractAddress, string code, Transaction transaction, TransactionReceipt transactionReceipt, bool failedCreatingContract, HexBigInteger blockTimestamp);

        Task UpsertAsync(Transaction transaction,
            TransactionReceipt transactionReceipt,
            bool failed,
            HexBigInteger timeStamp, bool hasVmStack = false, string error = null);
    }
}