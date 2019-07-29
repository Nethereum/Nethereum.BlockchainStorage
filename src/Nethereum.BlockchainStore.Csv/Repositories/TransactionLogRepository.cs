using CsvHelper.Configuration;
using Nethereum.BlockchainProcessing.BlockStorage.Entities;
using Nethereum.BlockchainProcessing.BlockStorage.Entities.Mapping;
using Nethereum.BlockchainProcessing.BlockStorage.Repositories;
using Nethereum.RPC.Eth.DTOs;
using System.Numerics;
using System.Threading.Tasks;

namespace Nethereum.BlockchainStore.Csv.Repositories
{
    public class TransactionLogRepository : CsvRepositoryBase<TransactionLog>, ITransactionLogRepository
    {
        public TransactionLogRepository(string csvFolderpath) : base(csvFolderpath, "TransactionLogs")
        {
        }

        public async Task<ITransactionLogView> FindByTransactionHashAndLogIndexAsync(string hash, BigInteger idx)
        {
            return await FindAsync(t => t.TransactionHash == hash && t.LogIndex == idx.ToString()).ConfigureAwait(false);
        }

        public async Task UpsertAsync(FilterLogVO log)
        {
            var transactionLog = log.MapToStorageEntityForUpsert();
            await Write(transactionLog).ConfigureAwait(false);
        }

        protected override ClassMap<TransactionLog> CreateClassMap()
        {
            return TransactionLogMap.Instance;
        }
    }

    public class TransactionLogMap : ClassMap<TransactionLog>
    {
        public static TransactionLogMap Instance = new TransactionLogMap();

        public TransactionLogMap()
        {
            AutoMap();
        }
    }
}
