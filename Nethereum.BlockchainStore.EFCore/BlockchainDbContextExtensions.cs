using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Nethereum.BlockchainStore.Entities;
using Nethereum.Hex.HexTypes;

namespace Nethereum.BlockchainStore.EFCore
{
    public static class BlockchainDbContextExtensions
    {
        public static async Task<T> FindByBlockNumberAndHashAsync<T>(this DbSet<T> transactions, HexBigInteger blockNumber, string transactionHash)
            where T: TransactionBase
        {
            return await transactions
                .SingleOrDefaultAsync(t => t.BlockNumber == blockNumber.Value.ToString() &&
                                           t.Hash == transactionHash);
        }

        public static async Task<Block> FindByBlockNumberAsync(this DbSet<Block> blocks, HexBigInteger blockNumber)
        {
            return await blocks
                .SingleOrDefaultAsync(t => t.BlockNumber == blockNumber.Value.ToString());
        }

        public static async Task<Contract> FindByContractAddressAsync(this DbSet<Contract> contracts, string contractAddress)
        {
            return await contracts
                .SingleOrDefaultAsync(c => c.Address == contractAddress);
        }

        public static async Task<TransactionLog> FindByTransactionHashAndLogIndexAsync(this DbSet<TransactionLog> transactionLogs, string transactionHash, long logIndex)
        {
            return await transactionLogs
                .SingleOrDefaultAsync(t => t.TransactionHash == transactionHash && t.LogIndex == logIndex);
        }

        public static async Task<TransactionVmStack> FindByTransactionHashAync(
            this DbSet<TransactionVmStack> transactionVmStacks, string transactionHash)
        {
            return await transactionVmStacks
                .SingleOrDefaultAsync(t => t.TransactionHash == transactionHash);
        }

    }
}