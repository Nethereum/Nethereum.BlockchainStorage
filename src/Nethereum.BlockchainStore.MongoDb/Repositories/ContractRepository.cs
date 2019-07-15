using System.Collections.Concurrent;
using System.Threading.Tasks;
using MongoDB.Driver;
using Nethereum.BlockchainStore.Entities;
using Nethereum.BlockchainStore.Entities.Mapping;
using Nethereum.BlockchainStore.MongoDb.Entities;
using Nethereum.BlockchainStore.Repositories;

namespace Nethereum.BlockchainStore.MongoDb.Repositories
{
    public class ContractRepository : MongoDbRepositoryBase<MongoDbContract>, IContractRepository
    {
        private readonly ConcurrentDictionary<string, MongoDbContract> _cachedContracts = new ConcurrentDictionary<string, MongoDbContract>();

        public ContractRepository(IMongoClient client, string databaseName) : base(client, databaseName, MongoDbCollectionName.Contracts)
        {
        }

        public async Task<bool> ExistsAsync(string contractAddress)
        {
            var filter = CreateDocumentFilter(contractAddress);

            var response = await Collection.Find(filter).SingleOrDefaultAsync();
            return response != null;
        }

        public async Task FillCache()
        {
            using (var cursor = await Collection.FindAsync(FilterDefinition<MongoDbContract>.Empty))
            {
                while (await cursor.MoveNextAsync())
                {
                    var batch = cursor.Current;
                    foreach (var contract in batch)
                        _cachedContracts.AddOrUpdate(contract.Address, contract, (s, existingContract) => contract);
                }
            }
        }

        public async Task<IContractView> FindByAddressAsync(string contractAddress)
        {
            var filter = CreateDocumentFilter(contractAddress);

            var response = await Collection.Find(filter).SingleOrDefaultAsync();
            return response;
        }

        public bool IsCached(string contractAddress)
        {
            return _cachedContracts.ContainsKey(contractAddress);
        }

        public async Task UpsertAsync(string contractAddress, string code, RPC.Eth.DTOs.Transaction transaction)
        {
            var contract = new MongoDbContract { };
            contract.Map(contractAddress, code, transaction);
            contract.UpdateRowDates();
            await UpsertDocumentAsync(contract);

            _cachedContracts.AddOrUpdate(contract.Address, contract,
                (s, existingContract) => contract);
        }
    }
}