using System.Threading.Tasks;
using MongoDB.Driver;
using Nethereum.BlockchainStore.Entities.Mapping;
using Nethereum.BlockchainStore.MongoDb.Entities;
using Nethereum.BlockchainStore.Repositories;
using Nethereum.Hex.HexTypes;
using Nethereum.RPC.Eth.DTOs;

namespace Nethereum.BlockchainStore.MongoDb.Repositories
{
    public class TransactionRepository : MongoDbRepositoryBase<MongoDbTransaction>, ITransactionRepository
    {
        public TransactionRepository(IMongoClient client, string databaseName) : base(client, databaseName, MongoDbCollectionName.Transactions)
        {
        }

        public async Task<BlockchainStore.Entities.ITransactionView> FindByBlockNumberAndHashAsync(HexBigInteger blockNumber, string hash)
        {
            var filter = CreateDocumentFilter(new MongoDbTransaction()
                {Hash = hash, BlockNumber = blockNumber.Value.ToString()});

            var response = await Collection.Find(filter).SingleOrDefaultAsync();
            return response;
        }

        public async Task UpsertAsync(string contractAddress, string code, RPC.Eth.DTOs.Transaction transaction, TransactionReceipt transactionReceipt, bool failedCreatingContract, HexBigInteger blockTimestamp)
        {
            var tx = new MongoDbTransaction();
            tx.Map(transaction);
            tx.Map(transactionReceipt);

            tx.NewContractAddress = contractAddress;
            tx.Failed = false;
            tx.TimeStamp = (long) blockTimestamp.Value;
            tx.Error = string.Empty;
            tx.HasVmStack = false;

            tx.UpdateRowDates();

            await UpsertDocumentAsync(tx);
        }

        public async Task UpsertAsync(RPC.Eth.DTOs.Transaction transaction, TransactionReceipt receipt, bool failed, HexBigInteger timeStamp, bool hasVmStack = false, string error = null)
        {
            var tx = new MongoDbTransaction();
            tx.Map(transaction);
            tx.Map(receipt);

            tx.Failed = failed;
            tx.TimeStamp = (long) timeStamp.Value;
            tx.Error = error ?? string.Empty;
            tx.HasVmStack = hasVmStack;

            tx.UpdateRowDates();

            await UpsertDocumentAsync(tx);
        }
    }
}