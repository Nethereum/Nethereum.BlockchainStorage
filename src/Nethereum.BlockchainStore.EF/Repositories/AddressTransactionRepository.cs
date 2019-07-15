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
    public class AddressTransactionRepository : RepositoryBase,  IAddressTransactionRepository
    {
        private readonly SemaphoreSlim _lock = new SemaphoreSlim(1);
        public AddressTransactionRepository(IBlockchainDbContextFactory contextFactory):base(contextFactory){}

        public async Task<IAddressTransactionView> FindAsync(string address, HexBigInteger blockNumber, string transactionHash)
        {
            using (var context = _contextFactory.CreateContext())
            {
                return await context.AddressTransactions.FindByBlockNumberAndHashAndAddressAsync
                    (blockNumber, transactionHash, address).ConfigureAwait(false);
            }
        }

        public async Task UpsertAsync(TransactionReceiptVO transactionReceiptVO, string address, string error = null, string newContractAddress = null)
        {
            await _lock.WaitAsync();
            try
            {
                using (var context = _contextFactory.CreateContext())
                {
                    var tx = await FindOrCreate(transactionReceiptVO.Transaction, address, context).ConfigureAwait(false);

                    tx.MapToStorageEntityForUpsert(transactionReceiptVO, address);
                    context.AddressTransactions.AddOrUpdate(tx);

                    await context.SaveChangesAsync().ConfigureAwait(false);
                }
            }
            finally
            {
                _lock.Release();
            }
        }

        private async Task<AddressTransaction> FindOrCreate(RPC.Eth.DTOs.Transaction transaction, string address, BlockchainDbContextBase context)
        {
            return await context.AddressTransactions.FindByBlockNumberAndHashAndAddressAsync
                       (transaction.BlockNumber, transaction.TransactionHash, address).ConfigureAwait(false)  ??
                   new AddressTransaction();
        }
    }
}
