using Microsoft.Extensions.Configuration;
using System;
using System.IO;

namespace Nethereum.BlockchainStore.EFCore
{
    public static class ConfigurationUtils
    {
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

        static string FindAppSettingsDirectory(Type type, string appSettingsFileName)
        {
            var assemblyFilePath = type.Assembly.Location;
            var startingDirectory = Path.GetDirectoryName(assemblyFilePath);
            return RecurseUntilFound(startingDirectory, appSettingsFileName);
        }

        public static IConfigurationRoot Build(Type type, string appSettingsFileName = "appsettings.json")
        {
            string path = FindAppSettingsDirectory(type, appSettingsFileName);

            if(path == null)
                throw new Exception("Failed to find the appsettings.json file.  Please ensure it is in somewhere within the path of the executable.");

            var config = new ConfigurationBuilder()
                .SetBasePath(path)
                .AddJsonFile(appSettingsFileName, optional: true, reloadOnChange: false)
                .AddEnvironmentVariables()
                .Build();

            return config;
        }

        public static void SetEnvironment(string environment)
        {
            Environment.SetEnvironmentVariable(AspnetcoreEnvironment, environment);
        }

        public static IConfigurationRoot Build(string[] args, string userSecretsId)
        {
            var environment = Environment.GetEnvironmentVariable(AspnetcoreEnvironment) ?? string.Empty;

            var baseJsonFile = "appsettings.json";
            var environmentJsonFile = $"appsettings.{environment}.json";

            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile(baseJsonFile, optional: true, reloadOnChange: false)
                .AddJsonFile(environmentJsonFile, optional: true, reloadOnChange: false)
                .AddEnvironmentVariables()
                .AddCommandLine(args);

            if (environment.Equals("development", StringComparison.InvariantCultureIgnoreCase))
                builder.AddUserSecrets(userSecretsId);

            return builder.Build();
        }
    }
}
