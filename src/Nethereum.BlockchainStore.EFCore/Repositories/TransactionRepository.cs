using Nethereum.BlockchainProcessing.Storage.Entities;
using Nethereum.BlockchainProcessing.Storage.Entities.Mapping;
using Nethereum.BlockchainProcessing.Storage.Repositories;
using Nethereum.Hex.HexTypes;
using Nethereum.RPC.Eth.DTOs;
using System.Threading.Tasks;

namespace Nethereum.BlockchainStore.EFCore.Repositories
{
    public class TransactionRepository : RepositoryBase, ITransactionRepository
    {
        public TransactionRepository(IBlockchainDbContextFactory contextFactory) : base(contextFactory){}

        private static async Task<BlockchainProcessing.Storage.Entities.Transaction> FindOrCreate(Nethereum.RPC.Eth.DTOs.Transaction transaction, BlockchainDbContextBase context)
        {
            return await context.Transactions
                         .FindByBlockNumberAndHashAsync(transaction.BlockNumber, transaction.TransactionHash).ConfigureAwait(false)  ??
                     new BlockchainProcessing.Storage.Entities.Transaction();
        }

        public async Task<ITransactionView> FindByBlockNumberAndHashAsync(HexBigInteger blockNumber, string hash)
        {
            using (var context = _contextFactory.CreateContext())
            {
                return await context.Transactions.FindByBlockNumberAndHashAsync(blockNumber, hash).ConfigureAwait(false);
            }
        }

        public async Task UpsertAsync(TransactionReceiptVO transactionReceiptVO, string code, bool failedCreatingContract)
        {
            using (var context = _contextFactory.CreateContext())
            {
                var tx = await FindOrCreate(transactionReceiptVO.Transaction, context).ConfigureAwait(false);

                tx.MapToStorageEntityForUpsert(transactionReceiptVO, code, failedCreatingContract);

                if (tx.IsNew())
                    context.Transactions.Add(tx);
                else
                    context.Transactions.Update(tx);

                await context.SaveChangesAsync().ConfigureAwait(false);
            }
        }

        public async Task UpsertAsync(TransactionReceiptVO transactionReceiptVO)
        {
            using (var context = _contextFactory.CreateContext())
            {
                var tx = await FindOrCreate(transactionReceiptVO.Transaction, context).ConfigureAwait(false);

                tx.MapToStorageEntityForUpsert(transactionReceiptVO);


                if (tx.IsNew())
                    context.Transactions.Add(tx);
                else
                    context.Transactions.Update(tx);

                await context.SaveChangesAsync().ConfigureAwait(false);
            }
        }
    }
}
