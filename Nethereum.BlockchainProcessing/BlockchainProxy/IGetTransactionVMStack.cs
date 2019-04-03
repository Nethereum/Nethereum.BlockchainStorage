using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace Nethereum.BlockchainProcessing.BlockchainProxy
{
    public interface IGetTransactionVMStack
    {
        Task<JObject> GetTransactionVmStack(string transactionHash);
    }
}
