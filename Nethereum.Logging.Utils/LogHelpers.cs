using cl = Common.Logging;
using mel = Microsoft.Extensions.Logging;

namespace Nethereum.Logging
{
    public static class LogHelpers
    {
        public static mel.LogLevel ToMicrosoftExtensionsLogging(this cl.LogLevel logLevel)
        {

            switch (logLevel)
            {
                case cl.LogLevel.All:
                    return mel.LogLevel.Trace;
                case cl.LogLevel.Debug:
                    return mel.LogLevel.Debug;
                case cl.LogLevel.Error:
                    return mel.LogLevel.Error;
                case cl.LogLevel.Fatal:
                    return mel.LogLevel.Critical;
                case cl.LogLevel.Info:
                    return mel.LogLevel.Information;
                case cl.LogLevel.Off:
                    return mel.LogLevel.None;
                case cl.LogLevel.Trace:
                    return mel.LogLevel.Trace;
                case cl.LogLevel.Warn:
                    return mel.LogLevel.Warning;
                default:
                    return mel.LogLevel.None;
            }

        }

    }
}
