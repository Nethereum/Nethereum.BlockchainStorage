using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Extensions.Configuration;
using Nethereum.BlockchainStore.CosmosCore.Repositories;
using Nethereum.BlockchainStore.EFCore;
using Nethereum.BlockchainStore.Processors;
using Nethereum.BlockchainStore.Repositories;
using System;
using System.Threading.Tasks;

namespace Nethereum.BlockchainStore.CosmosCore.Bootstrap
{
    public class CosmosRepositoryFactory : IBlockchainStoreRepositoryFactory
    {
        private readonly DocumentClient _client;
        private readonly string _databaseName;

        public CosmosRepositoryFactory(string endpointUri, string key, string prefix)
        {
            _databaseName = "BlockchainStorage" + prefix ?? string.Empty;
            _client = new DocumentClient(new Uri(endpointUri), key);
        }

        public async Task EnsureDatabaseAndCollections()
        {
            var db = await _client.CreateDatabaseIfNotExistsAsync(
                new Database {Id = _databaseName});

            var collectionsToCreate = Enum.GetNames(typeof(CosmosCollectionName));

            foreach (var collection in collectionsToCreate)
            {
                var docCollection = new DocumentCollection {Id = collection};
                await _client.CreateDocumentCollectionIfNotExistsAsync(db.Resource.SelfLink, docCollection);
            }
        }

        //TODO: Sort out how best to configure this - remove hard coded app settings file
        public static CosmosRepositoryFactory Create()
        {
            var config = ConfigurationUtils.Build(typeof(CosmosRepositoryFactory), "appsettings.development.js");

            var key = config.GetConnectionString("Cosmos");
            var endpointUri = config["CosmosEndpointUri"];
            var prefix = config["EnvironmentPrefix"];

            return new CosmosRepositoryFactory(endpointUri, key, prefix);
        }

        public IBlockRepository CreateBlockRepository()
        {
            return new BlockRepository(_client, _databaseName);
        }

        public IAddressTransactionRepository CreateAddressTransactionRepository()
        {
            return new AddressTransactionRepository(_client, _databaseName);
        }

        public IContractRepository CreateContractRepository()
        {
            return new ContractRepository(_client, _databaseName);
        }

        public ITransactionLogRepository CreateTransactionLogRepository()
        {
            return new TransactionLogRepository(_client, _databaseName);
        }

        public ITransactionVMStackRepository CreateTransactionVmStackRepository()
        {
            return new TransactionVMStackRepository(_client, _databaseName);
        }

        public ITransactionRepository CreateTransactionRepository()
        {
            return new TransactionRepository(_client, _databaseName);
        }
    }
}
