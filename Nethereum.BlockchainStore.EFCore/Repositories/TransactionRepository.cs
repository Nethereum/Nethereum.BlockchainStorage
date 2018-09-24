using Nethereum.BlockchainStore.Entities;
using Nethereum.BlockchainStore.Entities.Mapping;
using Nethereum.BlockchainStore.Repositories;
using Nethereum.Hex.HexTypes;
using Nethereum.RPC.Eth.DTOs;
using System.Threading.Tasks;
using Transaction = Nethereum.RPC.Eth.DTOs.Transaction;

namespace Nethereum.BlockchainStore.EFCore.Repositories
{
    public class TransactionRepository : RepositoryBase, ITransactionRepository
    {
        public TransactionRepository(IBlockchainDbContextFactory contextFactory) : base(contextFactory){}

        public async Task UpsertAsync(string contractAddress, string code, Transaction transaction, TransactionReceipt receipt, bool failedCreatingContract, HexBigInteger blockTimestamp)
        {
            using (var context = _contextFactory.CreateContext())
            {
                BlockchainStore.Entities.Transaction tx = await FindOrCreate(transaction, context).ConfigureAwait(false);

                tx.Map(transaction);
                tx.Map(receipt);

                tx.NewContractAddress = contractAddress;
                tx.Failed = false;
                tx.TimeStamp = (long)blockTimestamp.Value;
                tx.Error = string.Empty;
                tx.HasVmStack = false;

                tx.UpdateRowDates();

                if (tx.IsNew())
                    context.Transactions.Add(tx);
                else
                    context.Transactions.Update(tx);

                await context.SaveChangesAsync().ConfigureAwait(false) ;
            }
        }

        public async Task UpsertAsync(Transaction transaction, TransactionReceipt receipt, bool failed, HexBigInteger timeStamp, bool hasVmStack = false, string error = null)
        {
            using (var context = _contextFactory.CreateContext())
            {
                BlockchainStore.Entities.Transaction tx = await FindOrCreate(transaction, context).ConfigureAwait(false);

                tx.Map(transaction);
                tx.Map(receipt);

                tx.Failed = failed;
                tx.TimeStamp = (long)timeStamp.Value;
                tx.Error = error ?? string.Empty;
                tx.HasVmStack = hasVmStack;

                tx.UpdateRowDates();

                if (tx.IsNew())
                    context.Transactions.Add(tx);
                else
                    context.Transactions.Update(tx);

                await context.SaveChangesAsync().ConfigureAwait(false) ;
            }
        }

        private static async Task<BlockchainStore.Entities.Transaction> FindOrCreate(Transaction transaction, BlockchainDbContextBase context)
        {
            return await context.Transactions
                         .FindByBlockNumberAndHashAsync(transaction.BlockNumber, transaction.TransactionHash).ConfigureAwait(false)  ??
                     new BlockchainStore.Entities.Transaction();
        }

        public async Task<ITransactionView> FindByBlockNumberAndHashAsync(HexBigInteger blockNumber, string hash)
        {
            using (var context = _contextFactory.CreateContext())
            {
                return await context.Transactions.FindByBlockNumberAndHashAsync(blockNumber, hash).ConfigureAwait(false);
            }
        }
    }
}
