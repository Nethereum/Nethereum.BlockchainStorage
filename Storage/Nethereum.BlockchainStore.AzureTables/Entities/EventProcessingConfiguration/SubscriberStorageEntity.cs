using Nethereum.BlockchainProcessing.Processing.Logs.Configuration;

namespace Nethereum.BlockchainStore.AzureTables.Entities.EventProcessingConfiguration
{
    public class SubscriberStorageEntity : SubscriberOwnedBase, ISubscriberStorageDto
    {
        public bool Disabled { get; set; }

        public string Name { get; set; }
    }
}
