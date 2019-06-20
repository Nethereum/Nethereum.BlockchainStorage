using Nethereum.Geth;
using Newtonsoft.Json.Linq;

namespace Nethereum.BlockchainProcessing
{
    public class VmStackErrorCheckerWrapper: VmStackErrorChecker, IVmStackErrorChecker
    {
        string IVmStackErrorChecker.GetError(JObject stack)
        {
            var structsLogs = (JArray) stack["structLogs"];
            if (structsLogs.Count > 0)
            {
                var lastCall = structsLogs[structsLogs.Count - 1];
                return lastCall["error"]?.Value<string>();
            }
            return null;
        }
    }
}
