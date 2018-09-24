using CsvHelper.Configuration;
using Nethereum.BlockchainStore.Entities;
using Nethereum.BlockchainStore.Entities.Mapping;
using Nethereum.BlockchainStore.Repositories;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;

namespace Nethereum.BlockchainStore.Csv.Repositories
{
    public class TransactionLogRepository : CsvRepositoryBase<TransactionLog>, ITransactionLogRepository
    {
        public TransactionLogRepository(string csvFolderpath) : base(csvFolderpath, "TransactionLogs")
        {
        }

        public async Task<ITransactionLogView> FindByTransactionHashAndLogIndexAsync(string hash, long idx)
        {
            return await FindAsync(t => t.TransactionHash == hash && t.LogIndex == idx).ConfigureAwait(false);
        }

        public async Task UpsertAsync(string transactionHash, long logIndex, JObject log)
        {
            var transactionLog =  new TransactionLog();

            transactionLog.Map(transactionHash, logIndex, log);
            transactionLog.UpdateRowDates();

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
