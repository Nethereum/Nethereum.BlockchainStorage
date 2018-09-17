using System.Threading.Tasks;
using Nethereum.BlockchainStore.Entities;
using Newtonsoft.Json.Linq;

namespace Nethereum.BlockchainStore.Repositories
{
    public interface ITransactionVMStackRepository
    { 
        Task UpsertAsync(string transactionHash, string address, JObject stackTrace);
        Task<ITransactionVmStackView> FindByTransactionHashAync(string hash);
        Task<ITransactionVmStackView> FindByAddressAndTransactionHashAync(string address, string hash);
    }
}