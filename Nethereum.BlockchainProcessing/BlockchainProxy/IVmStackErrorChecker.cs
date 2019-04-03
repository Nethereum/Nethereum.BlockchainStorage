using Newtonsoft.Json.Linq;

namespace Nethereum.BlockchainProcessing.BlockchainProxy
{
    public interface IVmStackErrorChecker
    {
        string GetError(JObject stack);
        bool HasError(JObject stack);
    }
}
