using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using CsvHelper.Configuration;
using Nethereum.BlockchainStore.Entities;
using Nethereum.BlockchainStore.Entities.Mapping;
using Nethereum.BlockchainStore.Repositories;
using Newtonsoft.Json.Linq;

namespace Nethereum.BlockchainStore.Csv.Repositories
{
    public class TransactionVMStackRepository : CsvRepositoryBase<TransactionVmStack>, ITransactionVMStackRepository
    {
        public TransactionVMStackRepository(string csvFolderpath) : base(csvFolderpath, "TransactionVMStacks")
        {
        }

        public async Task<ITransactionVmStackView> FindByAddressAndTransactionHashAync(string address, string hash)
        {
            return await FindAsync(t => t.Address == address && t.TransactionHash == hash).ConfigureAwait(false);
        }

        public async Task<ITransactionVmStackView> FindByTransactionHashAync(string hash)
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

    public class TransactionVMStackMap : ClassMap<Entities.TransactionVmStack>
    {
        public static TransactionVMStackMap Instance = new TransactionVMStackMap();

        public TransactionVMStackMap()
        {
            AutoMap();
        }
    }
}
