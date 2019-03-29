using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;

namespace Nethereum.BlockchainProcessing.Processing
{
    public static class BlockchainSourceConfigurationFactory
    {
        public static string Default { get; } = "localhost";

        public static Dictionary<string, BlockchainSourceConfiguration> All = 
            new Dictionary<string, BlockchainSourceConfiguration>
        {
            {
                "localhost",
                new BlockchainSourceConfiguration(
                    blockchainUrl: "http://localhost:8545", 
                    name: "localhost"){FromBlock = 0}
            },
            {
                "rinkeby",
                new BlockchainSourceConfiguration(
                    blockchainUrl: "https://rinkeby.infura.io/v3/7238211010344719ad14a89db874158c", 
                    name: "rinkeby"){MinimumBlockNumber = 2830143}
            }
        };

        public static BlockchainSourceConfiguration Get(IConfigurationRoot config)
        {
            var presetName = config["Blockchain"] ?? Default;
            var configuration = Get(presetName);

            ApplyOverrides(config, configuration);

            return configuration;
        }

        private static void ApplyOverrides(
            IConfigurationRoot config, BlockchainSourceConfiguration configuration)
        {
            var minBlockNumber = ParseUlong(config, "MinimumBlockNumber");
            var minBlockConfirmations = ParseUint(config, "MinimumBlockConfirmations");
            var fromBlock = ParseUlong(config, "FromBlock");
            var toBlock = ParseUlong(config, "ToBlock");
            var blockchainUrl = config["BlockchainUrl"];
            var blockchainName = config["Blockchain"];
            var postVm = ParseBool(config, "PostVm");
            var processTransactionsInParallel = 
                ParseBool(config, "ProcessBlockTransactionsInParallel");

            if (minBlockNumber != null)
                configuration.MinimumBlockNumber = minBlockNumber;

            if (minBlockConfirmations != null)
                configuration.MinimumBlockConfirmations = minBlockConfirmations;

            if (fromBlock != null)
                configuration.FromBlock = fromBlock;

            if (toBlock != null)
                configuration.ToBlock = toBlock;

            if (blockchainUrl != null)
                configuration.BlockchainUrl = blockchainUrl;

            if (blockchainName != null)
                configuration.Name = blockchainName;

            if (postVm != null)
                configuration.PostVm = postVm.Value;

            if (processTransactionsInParallel != null)
                configuration.ProcessBlockTransactionsInParallel = 
                    processTransactionsInParallel.Value;
        }

        public static ulong? ParseUlong(IConfigurationRoot config, string name)
        {
            var configVal = config[name];

            return string.IsNullOrEmpty(configVal)
                ? (ulong?) null
                : ulong.Parse(configVal);
        }

        public static uint? ParseUint(IConfigurationRoot config, string name)
        {
            var configVal = config[name];

            return string.IsNullOrEmpty(configVal)
                ? (uint?) null
                : uint.Parse(configVal);
        }


        public static bool? ParseBool(IConfigurationRoot config, string name)
        {
            var configVal = config[name];

            return string.IsNullOrEmpty(configVal)
                ? (bool?) null
                : bool.Parse(configVal);
        }

        public static BlockchainSourceConfiguration Get(string name)
        {
            if(!All.ContainsKey(name))
                throw new Exception($"There is no preset configuration for '{name}'");

            return All[name];
        }
    }
}
