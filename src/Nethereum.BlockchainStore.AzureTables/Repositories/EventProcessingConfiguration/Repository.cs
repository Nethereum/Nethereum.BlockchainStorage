using Microsoft.WindowsAzure.Storage.Table;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Nethereum.BlockchainStore.AzureTables.Repositories.EventProcessingConfiguration
{
    public abstract class Repository<Dto, Entity> :
        AzureTableRepository<Entity>
        where Entity : class, ITableEntity, Dto, new() where Dto : class
    {
        public Repository(CloudTable table) : base(table)
        {
        }

        protected abstract Entity Map(Dto dto);

        public virtual async Task<Dto> UpsertAsync(Dto dto)
        {
            Entity e = dto as Entity ?? Map(dto);

            return await base.UpsertAsync(e).ConfigureAwait(false) as Dto;
        }

        public virtual async Task UpsertAsync(IEnumerable<Dto> dtos)
        {
            foreach (var dto in dtos)
            {
                await UpsertAsync(dto).ConfigureAwait(false);
            }
        }
    }
}
