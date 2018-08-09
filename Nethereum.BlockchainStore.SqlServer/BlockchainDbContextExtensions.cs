using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Nethereum.BlockchainStore.SqlServer.Entities;
using Nethereum.Hex.HexTypes;

namespace Nethereum.BlockchainStore.SqlServer
{
    public static class BlockchainDbContextExtensions
    {
        //public static async Task<AddressTransaction> FindByBlockNumberAndHashAsync(this DbSet<AddressTransaction> transactions, HexBigInteger blockNumber, string transactionHash)
        //{

        //    return await transactions
        //        .SingleOrDefaultAsync(t => t.BlockNumber == blockNumber.Value.ToString() &&
        //                             t.Hash == transactionHash);
        //}

        //public static async Task<Transaction> FindByBlockNumberAndHashAsync(this DbSet<Transaction> transactions, HexBigInteger blockNumber, string transactionHash)
        //{
        //    return await transactions
        //        .SingleOrDefaultAsync(t => t.BlockNumber == blockNumber.Value.ToString() &&
        //                                   t.Hash == transactionHash);
        //}

        public static async Task<T> FindByBlockNumberAndHashAsync<T>(this DbSet<T> transactions, HexBigInteger blockNumber, string transactionHash)
            where T: TransactionBase
        {
            return await transactions
                .SingleOrDefaultAsync(t => t.BlockNumber == blockNumber.Value.ToString() &&
                                           t.Hash == transactionHash);
        }
    }
}