using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace Nethereum.BlockchainStore.Repositories
{
    public interface ITransactionLogRepository
    {
        Task UpsertAsync(string transactionHash, long logIndex, JObject log);
    }
}