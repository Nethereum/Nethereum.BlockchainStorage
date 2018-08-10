using Nethereum.BlockchainStore.Repositories;
using Nethereum.BlockchainStore.SqlServer.Entities;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;

namespace Nethereum.BlockchainStore.SqlServer.Repositories
{
    public class TransactionVMStackRepository : RepositoryBase, ITransactionVMStackRepository
    {
        public TransactionVMStackRepository(IBlockchainDbContextFactory contextFactory) : base(contextFactory)
        {
        }

        public async Task UpsertAsync(string transactionHash, string address, JObject stackTrace)
        {
            using (var context = _contextFactory.CreateContext())
            {
                var record = await context
                                 .TransactionVmStacks
                                 .FindByTransactionHashAync(transactionHash).ConfigureAwait(false)  ??
                             new TransactionVmStack();

                MapValues(transactionHash, address, stackTrace, record);

                record.UpdateRowDates();

                if (record.IsNew())
                    context.TransactionVmStacks.Add(record);
                else
                    context.TransactionVmStacks.Update(record);

                await context.SaveChangesAsync().ConfigureAwait(false);
            }
        }

        private void MapValues(string transactionHash, string address, JObject stackTrace, Entities.TransactionVmStack transactionVmStack)
        {
            transactionVmStack.TransactionHash = transactionHash;
            transactionVmStack.Address = address;
            transactionVmStack.StructLogs = ((JArray) stackTrace["structLogs"]).ToString();
        }

    }
}
