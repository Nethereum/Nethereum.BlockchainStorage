using System.Threading.Tasks;
using Nethereum.BlockchainStore.Entities;
using Newtonsoft.Json.Linq;

namespace Nethereum.BlockchainStore.Repositories
{
    public interface ITransactionVMStackRepository
    { 
        Task UpsertAsync(string transactionHash, string address, JObject stackTrace);
    }

    public interface IEntityTransactionVMStackRepository : ITransactionVMStackRepository
    {
                Task<TransactionVmStack> FindByTransactionHashAync(string hash);
        Task Remove(TransactionVmStack transactionVmStack);
    }
}