using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Nethereum.BlockchainStore.CosmosCore.Repositories;
using Nethereum.BlockchainStore.EFCore;
using Nethereum.BlockchainStore.Processors;
using Nethereum.BlockchainStore.Repositories;
using System;
using System.Net;
using System.Threading.Tasks;

namespace Nethereum.BlockchainStore.CosmosCore.Bootstrap
{
    public class CosmosRepositoryFactory : IBlockchainStoreEntityRepositoryFactory
    {
        public static CosmosRepositoryFactory Create(string[] args = null, string userSecretsId = null, bool deleteAllExistingCollections = false)
        {
            var config = ConfigurationUtils.Build(args, userSecretsId);
       
            var endpointUri = config["CosmosEndpointUri"];

            if(string.IsNullOrEmpty(endpointUri))
                throw ConfigurationUtils.CreateKeyNotFoundException("CosmosEndpointUri");

            var key = config["CosmosAccessKey"];

            if(string.IsNullOrEmpty(endpointUri))
                throw ConfigurationUtils.CreateKeyNotFoundException("CosmosAccessKey");

            var tag = config["CosmosDbTag"];

            var factory = new CosmosRepositoryFactory(endpointUri, key, tag);

            var db = factory.CreateDbIfNotExistsAsync().Result;

            if (deleteAllExistingCollections)
                factory.DeleteAllCollections(db).Wait();

            factory.CreateCollectionsIfNotExist(db).Wait();

            return factory;
        }

        private readonly DocumentClient _client;
        private readonly string _databaseName;

        public CosmosRepositoryFactory(string endpointUri, string key, string dbTag)
        {
            _databaseName = "BlockchainStorage" + dbTag ?? string.Empty;
            _client = new DocumentClient(new Uri(endpointUri), key);
        }

        public async Task<Database> CreateDbIfNotExistsAsync()
        {
            var db = await _client.CreateDatabaseIfNotExistsAsync(
                new Database {Id = _databaseName});

            return db;
        }

        public async Task CreateCollectionsIfNotExist(Database db)
        {
            foreach (var collection in Enum.GetNames(typeof(CosmosCollectionName)))
            {
                var docCollection = new DocumentCollection {Id = collection};
                await _client.CreateDocumentCollectionIfNotExistsAsync(db.SelfLink, docCollection);
            }
        }

        public async Task DeleteAllCollections(Database db)
        {
            foreach (var collection in Enum.GetNames(typeof(CosmosCollectionName)))
            {
                var uri = UriFactory.CreateDocumentCollectionUri(db.Id, collection);

                try
                {
                    var existingCollection = await _client.ReadDocumentCollectionAsync(uri);

                    if (existingCollection.Resource != null)
                    {
                        await _client.DeleteDocumentCollectionAsync(uri);
                    }
                }
                catch (DocumentClientException dEx)
                {
                    if (dEx.StatusCode == HttpStatusCode.NotFound)
                        continue;

                }
            }
        }

        public IAddressTransactionRepository CreateAddressTransactionRepository()
        {
            return new AddressTransactionRepository(_client, _databaseName);
        }

        public IEntityBlockRepository CreateEntityBlockRepository()
        {
            return new BlockRepository(_client, _databaseName);
        }

        public IEntityContractRepository CreateEntityContractRepository()
        {
            return new ContractRepository(_client, _databaseName);
        }

        public IEntityTransactionLogRepository CreateEntityTransactionLogRepository()
        {
            return new TransactionLogRepository(_client, _databaseName);
        }

        public IEntityTransactionVMStackRepository CreateEntityTransactionVmStackRepository()
        {
            return new TransactionVMStackRepository(_client, _databaseName);
        }

        public IEntityTransactionRepository CreateEntityTransactionRepository()
        {
            return new TransactionRepository(_client, _databaseName);
        }

        public IBlockRepository CreateBlockRepository() => CreateEntityBlockRepository();
        public IContractRepository CreateContractRepository() => CreateEntityContractRepository();
        public ITransactionLogRepository CreateTransactionLogRepository() => CreateEntityTransactionLogRepository();
        public ITransactionRepository CreateTransactionRepository() => CreateEntityTransactionRepository();
        public ITransactionVMStackRepository CreateTransactionVmStackRepository() => CreateEntityTransactionVmStackRepository();
    }
}
