using Nethereum.BlockchainStore.AzureTables.Bootstrap;

namespace Nethereum.BlockchainProcessing.Processing.Logs
{
    public static class EventProcessingExtensions
    {
        public static EventLogProcessor UseAzureTableStorageForBlockProgress(
            this EventLogProcessor processor, string azureConnectionString, string tableNamePrefix)
        {
            return processor.UseAzureTableStorageForBlockProgress(new CloudTableSetup(azureConnectionString, tableNamePrefix));
        }

        public static EventLogProcessor UseAzureTableStorageForBlockProgress(
            this EventLogProcessor processor, CloudTableSetup cloudTableSetup)
        {
            processor.BlockProgressRepository = cloudTableSetup.CreateBlockProgressRepository();

            return processor;
        }
    }
}
