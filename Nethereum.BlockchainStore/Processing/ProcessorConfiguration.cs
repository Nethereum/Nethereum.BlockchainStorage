using System;
using Microsoft.Extensions.Configuration;
using Nethereum.BlockchainStore.EFCore;

namespace Nethereum.BlockchainStore.Processing
{
    public class ProcessorConfiguration
    {
        public ProcessorConfiguration(string blockchainUrl, string dbSchema)
        {
            BlockchainUrl = blockchainUrl;
            Schema = dbSchema;
        }

        public string BlockchainUrl { get; }
        public string Schema { get; }
        public long? MinimumBlockNumber { get; set; }
        public long? FromBlock { get; set; }
        public long? ToBlock { get; set; }
        public bool PostVm { get; set; } = false;
        public bool ProcessBlockTransactionsInParallel { get; set; } = true;

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
