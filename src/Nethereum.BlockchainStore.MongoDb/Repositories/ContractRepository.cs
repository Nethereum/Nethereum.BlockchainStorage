using MongoDB.Driver;
using Nethereum.BlockchainProcessing.BlockStorage.Entities;
using Nethereum.BlockchainProcessing.BlockStorage.Entities.Mapping;
using Nethereum.BlockchainProcessing.BlockStorage.Repositories;
using Nethereum.BlockchainStore.MongoDb.Entities;
using Nethereum.RPC.Eth.DTOs;
using System.Collections.Concurrent;
using System.Threading.Tasks;

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

            var response = await Collection.Find(filter).SingleOrDefaultAsync().ConfigureAwait(false);
            return response != null;
        }

        public async Task FillCache()
        {
            using (var cursor = await Collection.FindAsync(FilterDefinition<MongoDbContract>.Empty))
            {
                while (await cursor.MoveNextAsync().ConfigureAwait(false))
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

            var response = await Collection.Find(filter).SingleOrDefaultAsync().ConfigureAwait(false);
            return response;
        }

        public bool IsCached(string contractAddress)
        {
            return _cachedContracts.ContainsKey(contractAddress);
        }

        public async Task UpsertAsync(ContractCreationVO contractCreation)
        {
            var contract = contractCreation.MapToStorageEntityForUpsert<MongoDbContract>();
            await UpsertDocumentAsync(contract);

            _cachedContracts.AddOrUpdate(contract.Address, contract,
                (s, existingContract) => contract);
        }
    }
}