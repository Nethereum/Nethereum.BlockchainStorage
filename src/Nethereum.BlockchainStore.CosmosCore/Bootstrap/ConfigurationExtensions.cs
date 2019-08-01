using Microsoft.Extensions.Configuration;
using Nethereum.Microsoft.Configuration.Utils;

namespace Nethereum.BlockchainStore.CosmosCore.Bootstrap
{
    public static class ConfigurationExtensions
    {
        public static void SetCosmosAccessKey(this IConfigurationRoot appConfig, string cosmosAccessKey)
        {
            appConfig[ConfigurationKeyNames.CosmosAccessKey] = cosmosAccessKey;
        }

        public static void SetCosmosEndpointUri(this IConfigurationRoot appConfig, string cosmosEndpointUri)
        {
            appConfig[ConfigurationKeyNames.CosmosEndpointUri] = cosmosEndpointUri;
        }

        public static string GetCosmosAccessKeyOrThrow(this IConfigurationRoot config)
        {
            return config.GetOrThrow(ConfigurationKeyNames.CosmosAccessKey);
        }

        public static string GetCosmosEndpointUrlOrThrow(this IConfigurationRoot config)
        {
            return config.GetOrThrow(ConfigurationKeyNames.CosmosEndpointUri);
        }

        public static string GetCosmosDbTag(this IConfigurationRoot config)
        {
            return config[ConfigurationKeyNames.CosmosDbTag];
        }

    }
}
