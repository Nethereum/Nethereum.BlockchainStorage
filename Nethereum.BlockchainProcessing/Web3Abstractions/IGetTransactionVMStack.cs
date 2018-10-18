using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace Nethereum.BlockchainProcessing.Web3Abstractions
{
    public interface IGetTransactionVMStack
    {
        Task<JObject> GetTransactionVmStack(string transactionHash);
    }
}
