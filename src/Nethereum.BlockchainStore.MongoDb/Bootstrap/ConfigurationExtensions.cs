using Microsoft.Extensions.Configuration;
using Nethereum.Microsoft.Configuration.Utils;

namespace Nethereum.BlockchainStore.MongoDb.Bootstrap
{
    public static class ConfigurationExtensions
    {
        public static void SetMongoDbConnectionString(this IConfigurationRoot appConfig, string connectionString)
        {
            appConfig[ConfigurationKeyNames.MongoDbConnectionString] = connectionString;
        }

        public static string GetMongoDbConnectionStringOrThrow(this IConfigurationRoot config)
        {
            return config.GetOrThrow(ConfigurationKeyNames.MongoDbConnectionString);
        }

        public static string GetMongoDbTag(this IConfigurationRoot config)
        {
            return config[ConfigurationKeyNames.MongoDbTag];
        }

        public static string GetMongoDbLocale(this IConfigurationRoot config)
        {
            return config[ConfigurationKeyNames.MongoDbLocale] ?? "en";
        }
    }
}