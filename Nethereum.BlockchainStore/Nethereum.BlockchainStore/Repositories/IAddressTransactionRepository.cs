using System.Threading.Tasks;
using Nethereum.Hex.HexTypes;
using Nethereum.RPC.Eth.DTOs;

namespace Nethereum.BlockchainStore.Repositories
{
    public interface IAddressTransactionRepository
    {
        Task UpsertAsync(Transaction transaction, TransactionReceipt transactionReceipt, bool failedCreatingContract, HexBigInteger blockTimestamp, string address, string error = null, bool hasVmStack = false, string newContractAddress = null);
    }
}