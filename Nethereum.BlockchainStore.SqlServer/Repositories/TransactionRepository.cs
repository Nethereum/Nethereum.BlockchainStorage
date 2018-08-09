using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Nethereum.BlockchainStore.Repositories;
using Nethereum.Hex.HexTypes;
using Nethereum.RPC.Eth.DTOs;

namespace Nethereum.BlockchainStore.SqlServer.Repositories
{
    public class TransactionRepository : TransactionBaseRepository, ITransactionRepository
    {
        public TransactionRepository(IBlockchainDbContextFactory contextFactory) : base(contextFactory){}

        public async Task UpsertAsync(string contractAddress, string code, Transaction transaction, TransactionReceipt receipt, bool failedCreatingContract, HexBigInteger blockTimestamp)
        {
            using (var context = _contextFactory.CreateContext())
            {
                Entities.Transaction tx = await FindOrCreate(transaction, context);

                MapValues(transaction, tx);
                MapValues(receipt, tx);

                tx.NewContractAddress = contractAddress;
                tx.Failed = false;
                tx.TimeStamp = (long)blockTimestamp.Value;
                tx.Error = string.Empty;
                tx.HasVmStack = false;

                if (tx.IsNew())
                    context.Transactions.Add(tx);
                else
                    context.Transactions.Update(tx);

                await context.SaveChangesAsync();
            }
        }

        public async Task UpsertAsync(Transaction transaction, TransactionReceipt receipt, bool failed, HexBigInteger timeStamp, bool hasVmStack = false, string error = null)
        {
            using (var context = _contextFactory.CreateContext())
            {
                Entities.Transaction tx = await FindOrCreate(transaction, context);

                MapValues(transaction, tx);
                MapValues(receipt, tx);

                tx.Failed = failed;
                tx.TimeStamp = (long)timeStamp.Value;
                tx.Error = error ?? string.Empty;
                tx.HasVmStack = hasVmStack;

                if (tx.IsNew())
                    context.Transactions.Add(tx);
                else
                    context.Transactions.Update(tx);

                await context.SaveChangesAsync();
            }
        }

        private static async Task<Entities.Transaction> FindOrCreate(Transaction transaction, BlockchainDbContext context)
        {
            return await context.Transactions
                         .FindByBlockNumberAndHashAsync(transaction.BlockNumber, transaction.TransactionHash) ??
                     new Entities.Transaction();
        }

    }
}
