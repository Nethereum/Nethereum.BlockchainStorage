using Microsoft.Azure.Documents.Client;
using Nethereum.BlockchainStore.CosmosCore.Entities;
using System;
using System.Threading.Tasks;

namespace Nethereum.BlockchainStore.CosmosCore.Repositories
{
    public abstract class CosmosRepositoryBase
    {
        protected readonly string DatabaseName;
        protected readonly string CollectionName;
        protected readonly DocumentClient Client;

        protected CosmosRepositoryBase(DocumentClient client, string databaseName, CosmosCollectionName collectionName)
        {
            Client = client;
            DatabaseName = databaseName;
            CollectionName = collectionName.ToString();
        }

        protected async Task UpsertDocumentAsync(ICosmosEntity updatedDocument)
        {
            await Client.UpsertDocumentAsync(
                CreateCollectionUri(updatedDocument), 
                updatedDocument, options: null, disableAutomaticIdGeneration: true);
        }

        protected Uri CreateDocumentUri(ICosmosEntity entity)
        {
            return UriFactory.CreateDocumentUri(DatabaseName, CollectionName, entity.Id);
        }

        protected Uri CreateCollectionUri(ICosmosEntity entity)
        {
            return UriFactory.CreateDocumentCollectionUri(DatabaseName, CollectionName);
        }
    }
}
