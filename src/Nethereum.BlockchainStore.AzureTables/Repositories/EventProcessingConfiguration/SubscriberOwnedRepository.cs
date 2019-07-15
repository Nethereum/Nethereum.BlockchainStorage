using Microsoft.WindowsAzure.Storage.Table;
using Nethereum.BlockchainProcessing.Processing.Logs.Configuration;

namespace Nethereum.BlockchainStore.AzureTables.Repositories.EventProcessingConfiguration
{
    public abstract class SubscriberOwnedRepository<Dto, Entity> :
        OwnedRepository<Dto, Entity>, ISubscriberOwnedRepository<Dto>
        where Entity : class, ITableEntity, Dto, new() where Dto : class
    {
        public SubscriberOwnedRepository(CloudTable table) : base(table)
        {
        }
    }
}
