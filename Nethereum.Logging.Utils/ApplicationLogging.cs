using Microsoft.Extensions.Logging;

namespace Nethereum.Logging
{
    public static class ApplicationLogging
    {
        public static ILoggerFactory LoggerFactory {get;} = new LoggerFactory();
        public static ILogger CreateLogger<T>() => LoggerFactory.CreateLogger<T>();
        public static ILogger CreateConsoleLogger<T>() => LoggerFactory.AddConsole().CreateLogger<T>();

    }
}
