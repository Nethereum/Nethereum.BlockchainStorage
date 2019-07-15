using System.Numerics;
using System.Threading.Tasks;
using MongoDB.Driver;
using Nethereum.BlockchainStore.Entities.Mapping;
using Nethereum.BlockchainStore.MongoDb.Entities;
using Nethereum.BlockchainStore.Repositories;
using Nethereum.Hex.HexTypes;
using Nethereum.RPC.Eth.DTOs;

namespace Nethereum.BlockchainStore.MongoDb.Repositories
{
    public class BlockRepository : MongoDbRepositoryBase<MongoDbBlock>, IBlockRepository
    {
        public BlockRepository(IMongoClient client, string databaseName) : base(client, databaseName, MongoDbCollectionName.Blocks)
        {
        }

        public async Task<BlockchainStore.Entities.IBlockView> FindByBlockNumberAsync(HexBigInteger blockNumber)
        {
            var filter = CreateDocumentFilter(
                new MongoDbBlock() {Id = blockNumber.Value.ToString()});

            var response = await Collection.Find(filter).SingleOrDefaultAsync();
            return response;
        }

        public async Task<BigInteger?> GetMaxBlockNumberAsync()
        {
            var count = await Collection.CountDocumentsAsync(FilterDefinition<MongoDbBlock>.Empty);

            if (count == 0)
            {
                return null;
            }

            var max = await Collection.Find(FilterDefinition<MongoDbBlock>.Empty).Limit(1)
                .Sort(new SortDefinitionBuilder<MongoDbBlock>().Descending(block => block.BlockNumber))
                .Project(block => block.BlockNumber).SingleOrDefaultAsync();

            return BigInteger.Parse(max);
        }

        public async Task UpsertBlockAsync(Block source)
        {
            var block = new MongoDbBlock { };
            block.Map(source);
            block.UpdateRowDates();
            await UpsertDocumentAsync(block);
        }
    }
}