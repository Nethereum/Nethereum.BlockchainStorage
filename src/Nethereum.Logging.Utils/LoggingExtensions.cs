using Common.Logging;
using Microsoft.Extensions.Logging;

namespace Nethereum.Microsoft.Logging.Utils
{
    public static class LoggingExtensions
    {
        public static ILog ToILog(this ILogger logger) => new LogAdapter(logger);
    }
}
