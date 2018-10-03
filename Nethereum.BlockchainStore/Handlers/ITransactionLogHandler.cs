using Newtonsoft.Json.Linq;
using System.Threading.Tasks;

namespace Nethereum.BlockchainStore.Handlers
{
    public interface ITransactionLogHandler
    {
        Task HandleAsync(string transactionHash, long logIndex, JObject log);
    }
}
