using CsvHelper.Configuration;
using Nethereum.BlockchainProcessing.BlockStorage.Entities;
using Nethereum.BlockchainProcessing.BlockStorage.Entities.Mapping;
using Nethereum.BlockchainProcessing.BlockStorage.Repositories;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;

namespace Nethereum.BlockchainStore.Csv.Repositories
{
    public class TransactionVMStackRepository : CsvRepositoryBase<TransactionVmStack>, ITransactionVMStackRepository
    {
        public TransactionVMStackRepository(string csvFolderpath) : base(csvFolderpath, "TransactionVMStacks")
        {
        }

        public async Task<ITransactionVmStackView> FindByAddressAndTransactionHashAsync(string address, string hash)
        {
            return await FindAsync(t => t.Address == address && t.TransactionHash == hash).ConfigureAwait(false);
        }

        public async Task<ITransactionVmStackView> FindByTransactionHashAsync(string hash)
        {
            return await FindAsync(t => t.TransactionHash == hash).ConfigureAwait(false);
        }

        public async Task UpsertAsync(string transactionHash, string address, JObject stackTrace)
        {
            var record = new TransactionVmStack();
            record.Map(transactionHash, address, stackTrace);
            record.UpdateRowDates();
            await Write(record);
        }

        protected override ClassMap<TransactionVmStack> CreateClassMap()
        {
            return TransactionVMStackMap.Instance;
        }
    }

    public class TransactionVMStackMap : ClassMap<TransactionVmStack>
    {
        public static TransactionVMStackMap Instance = new TransactionVMStackMap();

        public TransactionVMStackMap()
        {
            AutoMap();
        }
    }
}
