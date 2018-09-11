using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
using Nethereum.BlockchainStore.CosmosCore.Entities;
using Nethereum.BlockchainStore.Entities.Mapping;
using Nethereum.BlockchainStore.Repositories;
using Nethereum.RPC.Eth.DTOs;
using System.Collections.Concurrent;
using System.Net;
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
                var uri = CreateDocumentUri(new CosmosContract {Id = contractAddress});
                var response = await Client.ReadDocumentAsync<CosmosContract>(uri);
                return response.Document != null;
            }
            catch (DocumentClientException dEx)
            {
                if (dEx.StatusCode == HttpStatusCode.NotFound)
                    return false;

                throw;
            }
        }

        public async Task FillCache()
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

        public bool IsCached(string contractAddress)
        {
            throw new System.NotImplementedException();
        }

        public async Task UpsertAsync(string contractAddress, string code, Transaction transaction)
        {
            var contract = new CosmosContract { };
            contract.Map(contractAddress, code, transaction);
            contract.UpdateRowDates();
            await UpsertDocumentAsync(contract);

            _cachedContracts.AddOrUpdate(contract.Address, contract,
                (s, existingContract) => contract);
        }
    }
}
