using Newtonsoft.Json.Linq;
using System.Threading.Tasks;

namespace Nethereum.BlockchainStore.Web3Abstractions
{
    public interface IGetTransactionVMStack
    {
        Task<JObject> GetTransactionVmStack(string transactionHash);
    }
}
