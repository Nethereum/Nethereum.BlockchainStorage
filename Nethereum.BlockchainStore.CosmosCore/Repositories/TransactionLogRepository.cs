using Microsoft.Azure.Documents.Client;
using Nethereum.BlockchainStore.CosmosCore.Entities;
using Nethereum.BlockchainStore.Entities;
using Nethereum.BlockchainStore.Entities.Mapping;
using Nethereum.BlockchainStore.Repositories;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.Documents;

namespace Nethereum.BlockchainStore.CosmosCore.Repositories
{
    public class TransactionLogRepository : CosmosRepositoryBase, ITransactionLogRepository
    {
        public TransactionLogRepository(DocumentClient client, string databaseName) : base(client, databaseName, CosmosCollectionName.TransactionLogs)
        {
        }

        public async Task<ITransactionLogView> FindByTransactionHashAndLogIndexAsync(string hash, long idx)
        {
            var uri = CreateDocumentUri(new CosmosTransactionLog{TransactionHash = hash, LogIndex = idx});
            try
            {
                var response = await Client.ReadDocumentAsync<CosmosTransactionLog>(uri);
                return response.Document;
            }
            catch (DocumentClientException dEx)
            {
                if (dEx.IsNotFound())
                    return null;

                throw;
            }
        }

        public async Task UpsertAsync(string transactionHash, long logIndex, JObject log)
        {
            var transactionLog = new CosmosTransactionLog(); { };
            transactionLog.Map(transactionHash, logIndex, log);
            transactionLog.UpdateRowDates();
            await UpsertDocumentAsync(transactionLog);
        }
    }
}
