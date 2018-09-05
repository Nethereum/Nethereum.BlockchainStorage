using Microsoft.Azure.Documents.Client;
using Nethereum.BlockchainStore.CosmosCore.Entities;
using Nethereum.BlockchainStore.Entities.Mapping;
using Nethereum.BlockchainStore.Repositories;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;

namespace Nethereum.BlockchainStore.CosmosCore.Repositories
{
    public class TransactionLogRepository : CosmosRepositoryBase, ITransactionLogRepository
    {
        public TransactionLogRepository(DocumentClient client, string databaseName) : base(client, databaseName, CosmosCollectionName.TransactionLogs)
        {
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
