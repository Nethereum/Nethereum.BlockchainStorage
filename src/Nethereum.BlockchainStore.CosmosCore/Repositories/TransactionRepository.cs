using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Nethereum.BlockchainProcessing.Storage.Entities;
using Nethereum.BlockchainProcessing.Storage.Entities.Mapping;
using Nethereum.BlockchainProcessing.Storage.Repositories;
using Nethereum.BlockchainStore.CosmosCore.Entities;
using Nethereum.Hex.HexTypes;
using Nethereum.RPC.Eth.DTOs;
using System.Threading.Tasks;

namespace Nethereum.BlockchainStore.CosmosCore.Repositories
{
    public class TransactionRepository : CosmosRepositoryBase, ITransactionRepository
    {
        public TransactionRepository(DocumentClient client, string databaseName) : base(client, databaseName, CosmosCollectionName.Transactions)
        {
        }

        public async Task<ITransactionView> FindByBlockNumberAndHashAsync(HexBigInteger blockNumber, string hash)
        {
            var uri = CreateDocumentUri(new CosmosTransaction(){Hash = hash, BlockNumber = blockNumber.Value.ToString()});
            try
            {
                var response = await Client.ReadDocumentAsync<CosmosTransaction>(uri);
                return response.Document;
            }
            catch (DocumentClientException dEx)
            {
                if (dEx.IsNotFound())
                    return null;

                throw;
            }
        }

        public async Task UpsertAsync(TransactionReceiptVO transactionReceiptVO, string code, bool failedCreatingContract)
        {
            var tx = transactionReceiptVO.MapToStorageEntityForUpsert<CosmosTransaction>(code, failedCreatingContract);
            await UpsertDocumentAsync(tx).ConfigureAwait(false);
        }

        public async Task UpsertAsync(TransactionReceiptVO transactionReceiptVO)
        {
            var tx = transactionReceiptVO.MapToStorageEntityForUpsert<CosmosTransaction>();
            await UpsertDocumentAsync(tx).ConfigureAwait(false);
        }
    }
}
