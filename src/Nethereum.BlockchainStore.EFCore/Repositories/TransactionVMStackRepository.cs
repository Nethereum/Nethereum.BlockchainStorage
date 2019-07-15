using Nethereum.BlockchainProcessing.Storage.Entities;
using Nethereum.BlockchainProcessing.Storage.Entities.Mapping;
using Nethereum.BlockchainProcessing.Storage.Repositories;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;

namespace Nethereum.BlockchainStore.EFCore.Repositories
{
    public class TransactionVMStackRepository : RepositoryBase, ITransactionVMStackRepository
    {
        public TransactionVMStackRepository(IBlockchainDbContextFactory contextFactory) : base(contextFactory)
        {
        }

        public async Task<ITransactionVmStackView> FindByAddressAndTransactionHashAync(string address, string hash)
        {
            using (var context = _contextFactory.CreateContext())
            {
                return await context.TransactionVmStacks.FindByAddressAndTransactionHashAync(address, hash).ConfigureAwait(false);
            }
        }

        public async Task<ITransactionVmStackView> FindByTransactionHashAync(string hash)
        {
            using (var context = _contextFactory.CreateContext())
            {
                return await context.TransactionVmStacks.FindByTransactionHashAync(hash).ConfigureAwait(false);
            }
        }

        public async Task Remove(TransactionVmStack transactionVmStack)
        {
            using (var context = _contextFactory.CreateContext())
            {
                var t = await context.TransactionVmStacks.FindByTransactionHashAync(transactionVmStack.TransactionHash)
                    .ConfigureAwait(false);
                if (t != null)
                {
                    context.TransactionVmStacks.Remove(t);
                    await context.SaveChangesAsync().ConfigureAwait(false);
                }
            }
        }

        public async Task UpsertAsync(string transactionHash, string address, JObject stackTrace)
        {
            using (var context = _contextFactory.CreateContext())
            {
                var record = await context
                                 .TransactionVmStacks
                                 .FindByTransactionHashAync(transactionHash).ConfigureAwait(false)  ??
                             new TransactionVmStack();

                record.MapToStorageEntityForUpsert(stackTrace, transactionHash, address);

                if (record.IsNew())
                    context.TransactionVmStacks.Add(record);
                else
                    context.TransactionVmStacks.Update(record);

                await context.SaveChangesAsync().ConfigureAwait(false);
            }
        }

    }
}
