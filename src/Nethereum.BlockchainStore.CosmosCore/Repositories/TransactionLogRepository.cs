using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Nethereum.BlockchainProcessing.Storage.Entities;
using Nethereum.BlockchainProcessing.Storage.Entities.Mapping;
using Nethereum.BlockchainProcessing.Storage.Repositories;
using Nethereum.BlockchainStore.CosmosCore.Entities;
using Nethereum.RPC.Eth.DTOs;
using System.Threading.Tasks;
using System.Numerics;

namespace Nethereum.BlockchainStore.CosmosCore.Repositories
{
    public class TransactionLogRepository : CosmosRepositoryBase, ITransactionLogRepository
    {
        public TransactionLogRepository(DocumentClient client, string databaseName) : base(client, databaseName, CosmosCollectionName.TransactionLogs)
        {
        }

        public async Task<ITransactionLogView> FindByTransactionHashAndLogIndexAsync(string hash, BigInteger idx)
        {
            var uri = CreateDocumentUri(new CosmosTransactionLog{TransactionHash = hash, LogIndex = idx.ToString()});
            try
            {
                var response = await Client.ReadDocumentAsync<CosmosTransactionLog>(uri).ConfigureAwait(false);
                return response.Document;
            }
            catch (DocumentClientException dEx)
            {
                if (dEx.IsNotFound())
                    return null;

                throw;
            }
        }

        public async Task UpsertAsync(FilterLogVO log)
        {
            var transactionLog = log.MapToStorageEntityForUpsert<CosmosTransactionLog>();
            await UpsertDocumentAsync(transactionLog).ConfigureAwait(false);
        }
    }
}
