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
    public class TransactionRepository : RepositoryBase, ITransactionRepository
    {
        private readonly SemaphoreSlim _lock = new SemaphoreSlim(1);

        public TransactionRepository(IBlockchainDbContextFactory contextFactory) : base(contextFactory)
        {

        }

        public async Task UpsertAsync(string contractAddress, string code, RPC.Eth.DTOs.Transaction transaction, TransactionReceipt receipt, bool failedCreatingContract, HexBigInteger blockTimestamp)
        {
            await _lock.WaitAsync();
            try
            {
                using (var context = _contextFactory.CreateContext())
                {
                    var tx = await FindOrCreate(transaction, context).ConfigureAwait(false);

                    tx.Map(transaction);
                    tx.Map(receipt);

                    tx.NewContractAddress = contractAddress;
                    tx.Failed = false;
                    tx.TimeStamp = (long) blockTimestamp.Value;
                    tx.Error = string.Empty;
                    tx.HasVmStack = false;

                    tx.UpdateRowDates();

                    context.Transactions.AddOrUpdate(tx);

                    await context.SaveChangesAsync().ConfigureAwait(false);
                }
            }
            finally
            {
                _lock.Release();
            }
        }

        public async Task UpsertAsync(RPC.Eth.DTOs.Transaction transaction, TransactionReceipt receipt, bool failed, HexBigInteger timeStamp, bool hasVmStack = false, string error = null)
        {
            await _lock.WaitAsync();
            try
            {
                using (var context = _contextFactory.CreateContext())
                {
                    var tx = await FindOrCreate(transaction, context).ConfigureAwait(false);

                    tx.Map(transaction);
                    tx.Map(receipt);

                    tx.Failed = failed;
                    tx.TimeStamp = (long) timeStamp.Value;
                    tx.Error = error ?? string.Empty;
                    tx.HasVmStack = hasVmStack;

                    tx.UpdateRowDates();

                    context.Transactions.AddOrUpdate(tx);

                    await context.SaveChangesAsync().ConfigureAwait(false);
                }
            }
            finally
            {
                _lock.Release();
            }
        }

        private async Task<Entities.Transaction> FindOrCreate(RPC.Eth.DTOs.Transaction transaction, BlockchainDbContextBase context)
        {
            return await context.Transactions.FindByBlockNumberAndHashAsync(transaction.BlockNumber, transaction.TransactionHash).ConfigureAwait(false)  ??
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
