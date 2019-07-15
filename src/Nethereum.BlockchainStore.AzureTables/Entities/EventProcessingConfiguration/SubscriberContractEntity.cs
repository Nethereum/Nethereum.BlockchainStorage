using Nethereum.BlockchainProcessing.Processing.Logs.Configuration;

namespace Nethereum.BlockchainStore.AzureTables.Entities.EventProcessingConfiguration
{
    public class SubscriberContractEntity : SubscriberOwnedBase, ISubscriberContractDto
    {
        public string Abi { get;set;}
        public string Name { get; set; }

    }
}
