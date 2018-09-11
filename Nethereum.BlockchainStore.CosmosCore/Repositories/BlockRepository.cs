using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
using Nethereum.BlockchainStore.CosmosCore.Entities;
using Nethereum.BlockchainStore.Entities;
using Nethereum.BlockchainStore.Entities.Mapping;
using Nethereum.BlockchainStore.Processors;
using Nethereum.Hex.HexTypes;
using Nethereum.RPC.Eth.DTOs;
using System.Linq;
using System.Threading.Tasks;

namespace Nethereum.BlockchainStore.CosmosCore.Repositories
{
    public class BlockRepository : CosmosRepositoryBase, IEntityBlockRepository
    {
        public BlockRepository(DocumentClient client, string databaseName) : base(client, databaseName, CosmosCollectionName.Blocks)
        {
        }

        public Task<BlockchainStore.Entities.Block> GetBlockAsync(HexBigInteger blockNumber)
        {
            throw new System.NotImplementedException();
        }

        public async Task<long> GetMaxBlockNumber()
        {
            var countQuery = await Client.CreateDocumentQuery<CosmosBlock>(
                UriFactory.CreateDocumentCollectionUri(DatabaseName, CollectionName)).CountAsync();

            if (countQuery == 0)
                return 0;

            var sqlQuery = "SELECT VALUE MAX(b.BlockNumber) FROM Blocks b";

            var query = Client.CreateDocumentQuery<long>(
                UriFactory.CreateDocumentCollectionUri(DatabaseName, CollectionName),
                sqlQuery).AsDocumentQuery();

            while (query.HasMoreResults)
            {
                var results = await query.ExecuteNextAsync();
                var result = results.AsEnumerable().First();
                return result;
            }

            return 0;
        }

        public async Task UpsertBlockAsync(BlockWithTransactionHashes source)
        {
            var block = new CosmosBlock { };
            block.Map(source);
            block.UpdateRowDates();
            await UpsertDocumentAsync(block);
        }
    }
}
