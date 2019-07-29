using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
using Nethereum.BlockchainProcessing.BlockStorage.Entities.Mapping;
using Nethereum.BlockchainProcessing.ProgressRepositories;
using Nethereum.BlockchainStore.CosmosCore.Entities;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;

namespace Nethereum.BlockchainStore.CosmosCore.Repositories
{
    public class BlockProgressRepository : CosmosRepositoryBase, IBlockProgressRepository
    {
        public BlockProgressRepository(DocumentClient client, string databaseName) : base(client, databaseName, CosmosCollectionName.BlockProgress)
        {
        }

        public async Task<BigInteger?> GetLastBlockNumberProcessedAsync()
        {
            var countQuery = await Client.CreateDocumentQuery<CosmosBlockProgress>(
                UriFactory.CreateDocumentCollectionUri(DatabaseName, CollectionName)).CountAsync().ConfigureAwait(false);

            if (countQuery == 0)
                return null;

            var sqlQuery = "SELECT VALUE MAX(b.LastBlockProcessed) FROM BlockProgress b";

            var query = Client.CreateDocumentQuery<BigInteger>(
                UriFactory.CreateDocumentCollectionUri(DatabaseName, CollectionName),
                sqlQuery).AsDocumentQuery();

            while (query.HasMoreResults)
            {
                var results = await query.ExecuteNextAsync().ConfigureAwait(false);
                var result = results.AsEnumerable().First();
                return result;
            }

            return 0;
        }

        public async Task UpsertProgressAsync(BigInteger blockNumber)
        {
            var block = blockNumber.MapToStorageEntityForUpsert<CosmosBlockProgress>();
            await UpsertDocumentAsync(block).ConfigureAwait(false);
        }
    }
}
