using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Nethereum.BlockchainProcessing.Storage.Entities;
using Nethereum.BlockchainProcessing.Storage.Entities.Mapping;
using Nethereum.BlockchainProcessing.Storage.Repositories;
using Nethereum.BlockchainStore.CosmosCore.Entities;
using Nethereum.Hex.HexTypes;
using System.Threading.Tasks;

namespace Nethereum.BlockchainStore.CosmosCore.Repositories
{
    public class BlockRepository : CosmosRepositoryBase, IBlockRepository
    {
        public BlockRepository(DocumentClient client, string databaseName) : base(client, databaseName, CosmosCollectionName.Blocks)
        {
        }

        public async Task<IBlockView> FindByBlockNumberAsync(HexBigInteger blockNumber)
        {
            var uri = CreateDocumentUri(new CosmosBlock() {Id = blockNumber.Value.ToString()});
            try
            {
                var response = await Client.ReadDocumentAsync<CosmosBlock>(uri).ConfigureAwait(false);
                return response.Document;
            }
            catch (DocumentClientException dEx)
            {
                if (dEx.IsNotFound())
                    return null;

                throw;
            }
        }

        public async Task UpsertBlockAsync(RPC.Eth.DTOs.Block source)
        {
            var block = source.MapToStorageEntityForUpsert<CosmosBlock>();
            await UpsertDocumentAsync(block).ConfigureAwait(false);
        }
    }
}
