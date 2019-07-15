using Microsoft.WindowsAzure.Storage.Table;
using Nethereum.BlockchainProcessing.Processing.Logs.Configuration;

namespace Nethereum.BlockchainStore.AzureTables.Entities.EventProcessingConfiguration
{
    public class SubscriberSearchIndexEntity : SubscriberOwnedBase, ISubscriberSearchIndexDto
    {
        public bool Disabled { get; set; }
        public string Name { get; set; }
    }
}
