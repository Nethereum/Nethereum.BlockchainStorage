using System.Threading.Tasks;
using Nethereum.BlockchainProcessing.Processing.Logs.Configuration;

namespace Nethereum.BlockchainStore.AzureTables.Entities.EventProcessingConfiguration
{

    public class SubscriberQueueEntity : SubscriberOwnedBase, ISubscriberQueueDto
    {
        public bool Disabled {get;set; }
        
        public string Name { get; set; }

    }
}
