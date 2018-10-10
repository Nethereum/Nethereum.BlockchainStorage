using System.Threading.Tasks;
using Nethereum.BlockchainStore.Entities;
using Nethereum.Hex.HexTypes;
using Nethereum.RPC.Eth.DTOs;
using Transaction = Nethereum.RPC.Eth.DTOs.Transaction;

namespace Nethereum.BlockchainStore.Repositories
{
    public interface IAddressTransactionRepository
    {
        Task UpsertAsync(
            Transaction transaction, TransactionReceipt transactionReceipt, bool failedCreatingContract, 
            HexBigInteger blockTimestamp, string address, string error = null, bool hasVmStack = false, 
            string newContractAddress = null);

        Task<IAddressTransactionView> FindAsync(
            string address, HexBigInteger blockNumber, string transactionHash);
    }
}