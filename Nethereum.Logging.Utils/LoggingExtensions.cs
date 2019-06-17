using Common.Logging;
using Nethereum.Logging;

namespace Microsoft.Extensions.Logging
{
    public static class LoggingExtensions
    {
        public static ILog ToILog(this ILogger logger) => new LogAdapter(logger);
    }
}
