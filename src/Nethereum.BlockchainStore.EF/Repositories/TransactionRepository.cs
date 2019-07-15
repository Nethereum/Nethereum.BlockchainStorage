using Nethereum.BlockchainProcessing.Storage.Entities;
using Nethereum.BlockchainProcessing.Storage.Entities.Mapping;
using Nethereum.BlockchainProcessing.Storage.Repositories;
using Nethereum.Hex.HexTypes;
using Nethereum.RPC.Eth.DTOs;
using System.Data.Entity.Migrations;
using System.Threading;
using System.Threading.Tasks;

namespace Nethereum.BlockchainStore.EF.Repositories
{
    public class TransactionRepository : RepositoryBase, ITransactionRepository
    {
        private readonly SemaphoreSlim _lock = new SemaphoreSlim(1);

        public TransactionRepository(IBlockchainDbContextFactory contextFactory) : base(contextFactory)
        {

        }

        private async Task<BlockchainProcessing.Storage.Entities.Transaction> FindOrCreate(RPC.Eth.DTOs.Transaction transaction, BlockchainDbContextBase context)
        {
            return await context.Transactions.FindByBlockNumberAndHashAsync(transaction.BlockNumber, transaction.TransactionHash).ConfigureAwait(false)  ??
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
            await _lock.WaitAsync();
            try
            {
                using (var context = _contextFactory.CreateContext())
                {
                    var tx = await FindOrCreate(transactionReceiptVO.Transaction, context).ConfigureAwait(false);

                    tx.MapToStorageEntityForUpsert(transactionReceiptVO, code, failedCreatingContract);

                    context.Transactions.AddOrUpdate(tx);

                    await context.SaveChangesAsync().ConfigureAwait(false);
                }
            }
            finally
            {
                _lock.Release();
            }
        }

        public async Task UpsertAsync(TransactionReceiptVO transactionReceiptVO)
        {
            await _lock.WaitAsync();
            try
            {
                using (var context = _contextFactory.CreateContext())
                {
                    var tx = await FindOrCreate(transactionReceiptVO.Transaction, context).ConfigureAwait(false);

                    tx.MapToStorageEntityForUpsert(transactionReceiptVO);

                    context.Transactions.AddOrUpdate(tx);

                    await context.SaveChangesAsync().ConfigureAwait(false);
                }
            }
            finally
            {
                _lock.Release();
            }
        }
    }
}
