using Newtonsoft.Json.Linq;
using System.Threading.Tasks;

namespace Nethereum.BlockchainStore.Handlers
{
    public interface ITransactionVMStackHandler
    {
        Task HandleAsync(string transactionHash, string address, JObject stackTrace);
    }
}
