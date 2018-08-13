using Microsoft.Extensions.Configuration;
using System;
using System.IO;

namespace Nethereum.BlockchainStore.EFCore
{
    public static class ConfigurationUtils
    {
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

        static string FindAppSettingsDirectory(Type type)
        {
            var assemblyFilePath = type.Assembly.Location;
            var startingDirectory = Path.GetDirectoryName(assemblyFilePath);
            return RecurseUntilFound(startingDirectory, "appsettings.json");
        }

        public static IConfigurationRoot Build(Type type)
        {
            string path = FindAppSettingsDirectory(type);

            if(path == null)
                throw new Exception("Failed to find the appsettings.json file.  Please ensure it is in somewhere within the path of the executable.");

            var config = new ConfigurationBuilder()
                .SetBasePath(path)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();

            return config;
        }
    }
}
