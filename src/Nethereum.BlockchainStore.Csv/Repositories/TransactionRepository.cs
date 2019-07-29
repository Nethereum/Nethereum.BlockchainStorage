using CsvHelper.Configuration;
using Nethereum.BlockchainProcessing.BlockStorage.Entities;
using Nethereum.BlockchainProcessing.BlockStorage.Entities.Mapping;
using Nethereum.BlockchainProcessing.BlockStorage.Repositories;
using Nethereum.Hex.HexTypes;
using Nethereum.RPC.Eth.DTOs;
using System.Threading.Tasks;
using Tx = Nethereum.BlockchainProcessing.BlockStorage.Entities.Transaction;

namespace Nethereum.BlockchainStore.Csv.Repositories
{

    public class TransactionRepository : CsvRepositoryBase<Tx>, ITransactionRepository
    {
        public TransactionRepository(string csvFolderpath) : base(csvFolderpath, "Transactions")
        {
        }

        public async Task<ITransactionView> FindByBlockNumberAndHashAsync(HexBigInteger blockNumber, string hash)
        {
            return await FindAsync(c => c.BlockNumber == blockNumber.Value.ToString() && c.Hash == hash).ConfigureAwait(false);
        }

        public async Task UpsertAsync(TransactionReceiptVO transactionReceiptVO, string code, bool failedCreatingContract)
        {
            var txEntity = transactionReceiptVO.MapToStorageEntityForUpsert(code, failedCreatingContract);
            await Write(txEntity).ConfigureAwait(false);
        }

        public async Task UpsertAsync(TransactionReceiptVO transactionReceiptVO)
        {
            var txEntity = transactionReceiptVO.MapToStorageEntityForUpsert();
            await Write(txEntity).ConfigureAwait(false);
        }

        protected override ClassMap<Tx> CreateClassMap()
        {
            return TransactionMap.Instance;
        }
    }

    public class TransactionMap : ClassMap<Tx>
    {
        public static TransactionMap Instance = new TransactionMap();

        public TransactionMap()
        {
            AutoMap();
        }
    }
}
