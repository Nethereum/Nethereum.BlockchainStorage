using Newtonsoft.Json.Linq;

namespace Nethereum.BlockchainProcessing
{
    public interface IVmStackErrorChecker
    {
        string GetError(JObject stack);
        bool HasError(JObject stack);
    }
}
