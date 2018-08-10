namespace Nethereum.BlockchainStore.SqlServer.Console
{
    partial class Program
    {
        public class ProcessorConfiguration
        {
            public ProcessorConfiguration(string blockchainUrl, string dbServer, string database, string dbSchema, string dbUserName, string dbPassword)
            {
                BlockchainUrl = blockchainUrl;
                Schema = dbSchema;
                ConnectionString = $"Data Source={dbServer};Database={database};Integrated Security=False;User ID={dbUserName};Password={dbPassword};Connect Timeout=60;";
            }

            public string BlockchainUrl { get; }
            public string ConnectionString { get; }
            public string Schema { get; }
            public long FromBlock { get; set; } = 0;
            public long ToBlock { get; set; } = 0;
            public bool PostVm { get; set; } = false;
        }
    }
}
