using System.Data.Entity;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Nethereum.BlockchainStore.Entities;
using Nethereum.Hex.HexTypes;

namespace Nethereum.BlockchainStore.EF
{
    public static class BlockchainDbContextExtensions
    {
        public static async Task<T> FindByBlockNumberAndHashAsync<T>(this IQueryable<T> transactions, HexBigInteger blockNumber, string transactionHash)
            where T: TransactionBase
        {
            var blockNumberString = blockNumber.Value.ToString();
            return await transactions
                .SingleOrDefaultAsync(t => t.BlockNumber == blockNumberString &&
                                           t.Hash == transactionHash);
        }

        public static async Task<Block> FindByBlockNumberAsync(this IQueryable<Block> blocks, HexBigInteger blockNumber)
        {
            var blockNumberString = blockNumber.Value.ToString();
            return await blocks
                .SingleOrDefaultAsync(t => t.BlockNumber == blockNumberString);
        }

        public static async Task<Contract> FindByContractAddressAsync(this IQueryable<Contract> contracts, string contractAddress)
        {
            return await contracts
                .SingleOrDefaultAsync(c => c.Address == contractAddress);
        }

        public static async Task<TransactionLog> FindByTransactionHashAndLogIndex(this IQueryable<TransactionLog> transactionLogs, string transactionHash, long logIndex)
        {
            return await transactionLogs
                .SingleOrDefaultAsync(t => t.TransactionHash == transactionHash && t.LogIndex == logIndex);
        }

        public static async Task<TransactionVmStack> FindByTransactionHashAync(
            this IQueryable<TransactionVmStack> transactionVmStacks, string transactionHash)
        {
            return await transactionVmStacks
                .SingleOrDefaultAsync(t => t.TransactionHash == transactionHash);
        }

    }
}