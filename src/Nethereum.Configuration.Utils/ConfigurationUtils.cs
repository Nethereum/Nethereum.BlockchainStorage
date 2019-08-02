using System;
using System.IO;
using Microsoft.Extensions.Configuration;

namespace Nethereum.Microsoft.Configuration.Utils
{
    public static class ConfigurationUtils
    {
        private const string DEVELOPMENT_ENVIRONMENT = "development";
        private const string ASPNETCORE_ENVIRONMENT_KEY = "ASPNETCORE_ENVIRONMENT";
        public static void SetEnvironmentAsDevelopment()
        {
            SetEnvironment(DEVELOPMENT_ENVIRONMENT);
        }

        public static void SetEnvironment(string environment)
        {
            Environment.SetEnvironmentVariable(ASPNETCORE_ENVIRONMENT_KEY, environment);
        }

        public static IConfigurationRoot Build(string appSettingsFileName = "appsettings.json")
        {
            string path = FindAppSettingsDirectory(appSettingsFileName);

            if (path == null && File.Exists(Path.Combine(Environment.CurrentDirectory, appSettingsFileName)))
                path = Environment.CurrentDirectory;

            if (path == null)
                throw new Exception($"Failed to find the appsettings.json file.  Please ensure it is in somewhere within the path of the executable.  Working Directory: {Environment.CurrentDirectory}");

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

            var environment = environmentBuilder[ASPNETCORE_ENVIRONMENT_KEY] ?? string.Empty;

            var baseJsonFile = "appsettings.json";
            var environmentJsonFile = $"appsettings.{environment}.json";

            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile(baseJsonFile, optional: true, reloadOnChange: false)
                .AddJsonFile(environmentJsonFile, optional: true, reloadOnChange: false);

            if (userSecretsId != null && environment.Equals("development", StringComparison.OrdinalIgnoreCase))
                builder.AddUserSecrets(userSecretsId);

            builder
                .AddEnvironmentVariables()
                .AddCommandLine(args ?? new string[] { });

            return builder.Build();
        }

        public static string GetOrThrow(this IConfigurationRoot config, string key)
        {
            var val = config[key];

            if (string.IsNullOrEmpty(val))
                throw CreateKeyNotFoundException(key);

            return val;
        }

        public static Exception CreateKeyNotFoundException(string key)
        {
            return new InvalidOperationException($"Value for configuration key '{key}' is empty - ensure it is set in appsettings.json OR appsettings.{{environment}}.json OR environment variables OR command line args OR user secrets");
        }


        private static string RecurseUntilFound(string directory, string fileToFind)
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

        private static string FindAppSettingsDirectory(string appSettingsFileName)
        {
            var assemblyFilePath = Directory.GetCurrentDirectory();
            var startingDirectory = Path.GetDirectoryName(assemblyFilePath);
            return RecurseUntilFound(startingDirectory, appSettingsFileName);
        }


    }
}
