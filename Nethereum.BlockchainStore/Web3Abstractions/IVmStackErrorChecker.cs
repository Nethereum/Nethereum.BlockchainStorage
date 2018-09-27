using Newtonsoft.Json.Linq;

namespace Nethereum.BlockchainStore.Web3Abstractions
{
    public interface IVmStackErrorChecker
    {
        string GetError(JObject stack);
        bool HasError(JObject stack);
    }
}
