using Nethereum.BlockchainProcessing.BlockStorage.Entities;
using Nethereum.Hex.HexTypes;
using System.Data.Entity;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;

namespace Nethereum.BlockchainStore.EF
{
    public static class BlockchainDbContextExtensions
    {
        public static async Task<T> FindByBlockNumberAndHashAsync<T>(this IQueryable<T> transactions, HexBigInteger blockNumber, string transactionHash)
            where T: TransactionBase
        {
            var blockNumberString = blockNumber.Value.ToString();
            return await transactions.AsNoTracking()
                .SingleOrDefaultAsync(t => t.BlockNumber == blockNumberString &&
                                           t.Hash == transactionHash);
        }

        public static async Task<T> FindByBlockNumberAndHashAndAddressAsync<T>(this IQueryable<T> transactions, HexBigInteger blockNumber, string transactionHash, string address)
            where T: AddressTransaction
        {
            var blockNumberString = blockNumber.Value.ToString();
            return await transactions.AsNoTracking()
                .SingleOrDefaultAsync(t => t.BlockNumber == blockNumberString &&
                                           t.Hash == transactionHash && t.Address == address);
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

        public static async Task<TransactionLog> FindByTransactionHashAndLogIndex(this IQueryable<TransactionLog> transactionLogs, string transactionHash, BigInteger logIndex)
        {
            var logIndexString = logIndex.ToString();

            return await transactionLogs
                .SingleOrDefaultAsync(t => t.TransactionHash == transactionHash && t.LogIndex == logIndexString);
        }

        public static async Task<TransactionVmStack> FindByTransactionHashAync(
            this IQueryable<TransactionVmStack> transactionVmStacks, string transactionHash)
        {
            return await transactionVmStacks
                .SingleOrDefaultAsync(t => t.TransactionHash == transactionHash);
        }

        public static async Task<TransactionVmStack> FindByAddressAndTransactionHashAync(
            this IQueryable<TransactionVmStack> transactionVmStacks, string address, string transactionHash)
        {
            return await transactionVmStacks
                .SingleOrDefaultAsync(t => t.TransactionHash == transactionHash && t.Address == address);
        }

    }
}