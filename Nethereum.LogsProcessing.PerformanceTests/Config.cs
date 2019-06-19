using Microsoft.Extensions.Configuration;
using Nethereum.Configuration;
using System;

namespace Nethereum.BlockchainProcessing.PerformanceTests
{
    public class Config
    {
        static IConfigurationRoot _config;


        public static IConfigurationRoot Configuration
        {
            get
            {
                if(_config == null)
                {
                    ConfigurationUtils.SetEnvironmentAsDevelopment();

                    //use the command line to set your azure search api key
                    //e.g. dotnet user-secrets set "AzureStorageConnectionString" "<put key here>"
                    _config = ConfigurationUtils
                        .Build(Array.Empty<string>(), userSecretsId: "Nethereum.BlockchainProcessing.PerformanceTests");
                }
                return _config;
            }
        }

        public static string AzureConnectionString => Configuration["AzureStorageConnectionString"];
    }
}
