using Common.Logging;
using Nethereum.Logging;

namespace Microsoft.Extensions.Logging
{
    public static class CommonLoggingExtensions
    {
        public static ILog ToILog(this ILogger logger) => new CommonLoggingAdapter(logger);
    }
}
