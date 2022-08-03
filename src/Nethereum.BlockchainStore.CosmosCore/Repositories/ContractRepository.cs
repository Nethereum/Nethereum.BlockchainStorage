using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
using Nethereum.BlockchainProcessing.BlockStorage.Entities;
using Nethereum.BlockchainProcessing.BlockStorage.Entities.Mapping;
using Nethereum.BlockchainProcessing.BlockStorage.Repositories;
using Nethereum.BlockchainStore.CosmosCore.Entities;
using Nethereum.RPC.Eth.DTOs;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace Nethereum.BlockchainStore.CosmosCore.Repositories
{
    public class ContractRepository : CosmosRepositoryBase, IContractRepository
    {
        private readonly ConcurrentDictionary<string, CosmosContract> _cachedContracts = new ConcurrentDictionary<string, CosmosContract>();

        public ContractRepository(DocumentClient client, string databaseName) : base(client, databaseName, CosmosCollectionName.Contracts)
        {
        }

        public async Task<bool> ExistsAsync(string contractAddress)
        {
            try
            {
                var uri = CreateDocumentUri(contractAddress);
                var response = await Client.ReadDocumentAsync<CosmosContract>(uri);
                return response.Document != null;
            }
            catch (DocumentClientException dEx)
            {
                if (dEx.IsNotFound())
                    return false;

                throw;
            }
        }

        public async Task FillCacheAsync()
        {
            FeedOptions queryOptions = new FeedOptions { MaxItemCount = -1 };

            var contractQuery = Client.CreateDocumentQuery<CosmosContract>(
                    UriFactory.CreateDocumentCollectionUri(DatabaseName, CollectionName), queryOptions)
                .AsDocumentQuery();

            while (contractQuery.HasMoreResults)
            {
                var contractResult = await contractQuery.ExecuteNextAsync<CosmosContract>();
                foreach (var contract in contractResult)
                {
                    _cachedContracts.AddOrUpdate(contract.Address, contract,
                        (s, existingContract) => contract);
                }
            }
        }

        public async Task<IContractView> FindByAddressAsync(string contractAddress)
        {
            var uri = CreateDocumentUri(contractAddress);
            try
            {
                var response = await Client.ReadDocumentAsync<CosmosContract>(uri).ConfigureAwait(false);
                return response.Document;
            }
            catch (DocumentClientException dEx)
            {
                if (dEx.IsNotFound())
                    return null;

                throw;
            }
        }

        public bool IsCached(string contractAddress)
        {
            return _cachedContracts.ContainsKey(contractAddress);
        }

        public async Task UpsertAsync(ContractCreationVO contractCreation)
        {
            var contract = contractCreation.MapToStorageEntityForUpsert<CosmosContract>();
            await UpsertDocumentAsync(contract).ConfigureAwait(false);

            _cachedContracts.AddOrUpdate(contract.Address, contract,
                (s, existingContract) => contract);
        }
    }
}
