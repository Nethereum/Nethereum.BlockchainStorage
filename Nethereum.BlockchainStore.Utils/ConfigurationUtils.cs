using Microsoft.Extensions.Configuration;
using System;
using System.IO;

namespace Nethereum.BlockchainStore
{
    public static class ConfigurationUtils
    {
        public static Exception CreateKeyNotFoundException(string key)
        {
            return new InvalidOperationException($"Value for configuration key '{key}' is empty - ensure it is set in appsettings.json OR appsettings.{{environment}}.json OR environment variables OR command line args OR user secrets");
        }

        private const string AspnetcoreEnvironment = "ASPNETCORE_ENVIRONMENT";

        static string RecurseUntilFound(string directory, string fileToFind)
        {
            if (Directory.GetFiles(directory, fileToFind).Length > 0)
            {
                return directory;
            }

            var parent = Directory.GetParent(directory);
            if (parent == null)
                return null;

            return RecurseUntilFound(parent.FullName, fileToFind);
        }

        static string FindAppSettingsDirectory(string appSettingsFileName)
        {
            var assemblyFilePath = Directory.GetCurrentDirectory();
            var startingDirectory = Path.GetDirectoryName(assemblyFilePath);
            return RecurseUntilFound(startingDirectory, appSettingsFileName);
        }

        public static void SetEnvironment(string environment)
        {
            Environment.SetEnvironmentVariable(AspnetcoreEnvironment, environment);
        }

        public static IConfigurationRoot Build(string appSettingsFileName = "appsettings.json")
        {
            string path = FindAppSettingsDirectory(appSettingsFileName);

            if(path == null)
                throw new Exception("Failed to find the appsettings.json file.  Please ensure it is in somewhere within the path of the executable.");

            var config = new ConfigurationBuilder()
                .SetBasePath(path)
                .AddJsonFile(appSettingsFileName, optional: true, reloadOnChange: false)
                .AddEnvironmentVariables()
                .Build();

            return config;
        }

        public static IConfigurationRoot Build(string[] args, string userSecretsId = null)
        {
            var environmentBuilder = new ConfigurationBuilder()
                .AddEnvironmentVariables()
                .AddCommandLine(args ?? new string[] { })
                .Build();

            var environment = environmentBuilder[AspnetcoreEnvironment] ?? string.Empty;

            var baseJsonFile = "appsettings.json";
            var environmentJsonFile = $"appsettings.{environment}.json";

            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile(baseJsonFile, optional: true, reloadOnChange: false)
                .AddJsonFile(environmentJsonFile, optional: true, reloadOnChange: false);

            if (userSecretsId != null && environment.Equals("development", StringComparison.InvariantCultureIgnoreCase))
                builder.AddUserSecrets(userSecretsId);

            builder
                .AddEnvironmentVariables()
                .AddCommandLine(args ?? new string[]{});

            return builder.Build();
        }
    }
}
