using CsvHelper.Configuration;
using Nethereum.BlockchainStore.Entities;
using Nethereum.BlockchainStore.Entities.Mapping;
using Nethereum.BlockchainStore.Repositories;
using Nethereum.Hex.HexTypes;
using Nethereum.RPC.Eth.DTOs;
using System.Threading.Tasks;

namespace Nethereum.BlockchainStore.Csv.Repositories
{
    public class TransactionRepository : CsvRepositoryBase<Entities.Transaction>, ITransactionRepository
    {
        public TransactionRepository(string csvFolderpath) : base(csvFolderpath, "Transactions")
        {
        }

        public async Task<ITransactionView> FindByBlockNumberAndHashAsync(HexBigInteger blockNumber, string hash)
        {
            return await FindAsync(c => c.BlockNumber == blockNumber.Value.ToString() && c.Hash == hash).ConfigureAwait(false);
        }

        public async Task UpsertAsync(string contractAddress, string code, RPC.Eth.DTOs.Transaction transaction, TransactionReceipt transactionReceipt, bool failedCreatingContract, HexBigInteger blockTimestamp)
        {
            Entities.Transaction tx = new Entities.Transaction();

            tx.Map(transaction);
            tx.Map(transactionReceipt);

            tx.NewContractAddress = contractAddress;
            tx.Failed = false;
            tx.TimeStamp = (long)blockTimestamp.Value;
            tx.Error = string.Empty;
            tx.HasVmStack = false;

            tx.UpdateRowDates();

            await Write(tx);
        }

        public async Task UpsertAsync(RPC.Eth.DTOs.Transaction transaction, TransactionReceipt receipt, bool failed, HexBigInteger timeStamp, bool hasVmStack = false, string error = null)
        {
            Entities.Transaction tx = new Entities.Transaction();

            tx.Map(transaction);
            tx.Map(receipt);

            tx.Failed = failed;
            tx.TimeStamp = (long)timeStamp.Value;
            tx.Error = error ?? string.Empty;
            tx.HasVmStack = hasVmStack;

            tx.UpdateRowDates();

            await Write(tx);
        }

        protected override ClassMap<Entities.Transaction> CreateClassMap()
        {
            return TransactionMap.Instance;
        }

    }

    public class TransactionMap : ClassMap<Entities.Transaction>
    {
        public static TransactionMap Instance = new TransactionMap();

        public TransactionMap()
        {
            AutoMap();
        }
    }
}
