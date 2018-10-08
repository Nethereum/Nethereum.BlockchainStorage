using Newtonsoft.Json.Linq;
using System.Threading.Tasks;

namespace Nethereum.BlockchainStore.Handlers
{
    public interface ITransactionLogHandler
    {
        Task HandleAsync(string transactionHash, long logIndex, JObject log);
    }

    public class NullTransactionLogHandler : ITransactionLogHandler
    {
        public Task HandleAsync(string transactionHash, long logIndex, JObject log)
        {
            return Task.CompletedTask;
        }
    }
}
