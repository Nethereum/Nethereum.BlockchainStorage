using MongoDB.Driver;
using Nethereum.BlockchainProcessing.BlockStorage.Entities;
using Nethereum.BlockchainProcessing.BlockStorage.Entities.Mapping;
using Nethereum.BlockchainProcessing.BlockStorage.Repositories;
using Nethereum.BlockchainStore.MongoDb.Entities;
using Nethereum.Hex.HexTypes;
using System.Numerics;
using System.Threading.Tasks;

namespace Nethereum.BlockchainStore.MongoDb.Repositories
{
    public class BlockRepository : MongoDbRepositoryBase<MongoDbBlock>, IBlockRepository
    {
        public BlockRepository(IMongoClient client, string databaseName) : base(client, databaseName, MongoDbCollectionName.Blocks)
        {
        }

        public async Task<IBlockView> FindByBlockNumberAsync(HexBigInteger blockNumber)
        {
            var filter = CreateDocumentFilter(
                new MongoDbBlock() {Id = blockNumber.Value.ToString()});

            var response = await Collection.Find(filter).SingleOrDefaultAsync().ConfigureAwait(false);
            return response;
        }

        public async Task<BigInteger?> GetMaxBlockNumberAsync()
        {
            var count = await Collection.CountDocumentsAsync(FilterDefinition<MongoDbBlock>.Empty).ConfigureAwait(false);

            if (count == 0)
            {
                return null;
            }

            var max = await Collection.Find(FilterDefinition<MongoDbBlock>.Empty).Limit(1)
                .Sort(new SortDefinitionBuilder<MongoDbBlock>().Descending(block => block.BlockNumber))
                .Project(block => block.BlockNumber).SingleOrDefaultAsync().ConfigureAwait(false);

            return BigInteger.Parse(max);
        }

        public async Task UpsertBlockAsync(RPC.Eth.DTOs.Block source)
        {
            var block = source.MapToStorageEntityForUpsert<MongoDbBlock>();
            await UpsertDocumentAsync(block).ConfigureAwait(false);
        }
    }
}