﻿using System;
using Microsoft.Extensions.Configuration;
using Nethereum.BlockchainStore.EFCore;

namespace Nethereum.BlockchainStore.Sqlite.Console
{
    public class ProcessorConfiguration
    {
        public ProcessorConfiguration(string blockchainUrl, string dbSchema)
        {
            BlockchainUrl = blockchainUrl;
            Schema = dbSchema;
        }

        public string BlockchainUrl { get; }
        public long FromBlock { get; set; } = 0;
        public long ToBlock { get; set; } = 0;
        public bool PostVm { get; set; } = false;
        public string Schema { get;set; }

        public string GetConnectionString()
        {
            var config = ConfigurationUtils.Build(this.GetType());
            var connectionStringName = $"BlockchainDbStorage_{Schema}";
            var connectionString = config.GetConnectionString(connectionStringName);
            if (string.IsNullOrEmpty(connectionString))
                throw new Exception($"Null or empty connection string for schema: {Schema} and connection string name: {connectionStringName}");

            return connectionString;
        }
    }
}
