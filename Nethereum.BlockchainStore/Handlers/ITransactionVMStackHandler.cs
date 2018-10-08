using Newtonsoft.Json.Linq;
using System.Threading.Tasks;

namespace Nethereum.BlockchainStore.Handlers
{
    public interface ITransactionVMStackHandler
    {
        Task HandleAsync(string transactionHash, string address, JObject stackTrace);
    }

    public class NullTransactionVMStackHandler : ITransactionVMStackHandler
    {
        public Task HandleAsync(string transactionHash, string address, JObject stackTrace)
        {
            return Task.CompletedTask;
        }
    }
}
