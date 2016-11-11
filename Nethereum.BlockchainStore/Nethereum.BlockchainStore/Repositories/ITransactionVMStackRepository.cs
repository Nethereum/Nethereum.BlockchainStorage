using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace Nethereum.BlockchainStore.Repositories
{
    public interface ITransactionVMStackRepository
    {
        Task UpsertAsync(string transactionHash, string address, JObject stackTrace);
    }
}