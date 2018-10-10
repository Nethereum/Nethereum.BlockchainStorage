using System.Data.Entity.Migrations;
using System.Threading;
using System.Threading.Tasks;
using Nethereum.BlockchainStore.Entities;
using Nethereum.BlockchainStore.Entities.Mapping;
using Nethereum.BlockchainStore.Repositories;
using Nethereum.Hex.HexTypes;
using Nethereum.RPC.Eth.DTOs;

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

        public async Task UpsertAsync(
            RPC.Eth.DTOs.Transaction transaction, 
            TransactionReceipt receipt, 
            bool failedCreatingContract, 
            HexBigInteger blockTimestamp, 
            string address, 
            string error = null, 
            bool hasVmStack = false, 
            string newContractAddress = null)
        {

            await _lock.WaitAsync();
            try
            {
                using (var context = _contextFactory.CreateContext())
                {
                    var tx = await FindOrCreate(transaction, address, context).ConfigureAwait(false);

                    tx.Map(transaction, address);
                    tx.UpdateRowDates();
                    context.AddressTransactions.AddOrUpdate(tx);

                    await context.SaveChangesAsync().ConfigureAwait(false);
                }
            }
            finally
            {
                _lock.Release();
            }
        }

        private async Task<Entities.AddressTransaction> FindOrCreate(RPC.Eth.DTOs.Transaction transaction, string address, BlockchainDbContextBase context)
        {
            return await context.AddressTransactions.FindByBlockNumberAndHashAndAddressAsync
                       (transaction.BlockNumber, transaction.TransactionHash, address).ConfigureAwait(false)  ??
                   new BlockchainStore.Entities.AddressTransaction();
        }
    }
}
