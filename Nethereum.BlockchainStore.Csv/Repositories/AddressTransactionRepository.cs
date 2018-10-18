using Nethereum.BlockchainStore.Entities;
using Nethereum.BlockchainStore.Repositories;
using Nethereum.Hex.HexTypes;
using Nethereum.RPC.Eth.DTOs;
using System.Threading.Tasks;
using Nethereum.BlockchainStore.Entities.Mapping;
using CsvHelper.Configuration;

namespace Nethereum.BlockchainStore.Csv.Repositories
{
    public class AddressTransactionRepository : CsvRepositoryBase<Entities.AddressTransaction>, IAddressTransactionRepository
    {
        public AddressTransactionRepository(string csvFolderpath) : base(csvFolderpath, "AddressTransactions")
        {
        }

        public async Task<IAddressTransactionView> FindAsync(string address, HexBigInteger blockNumber, string transactionHash)
        {
            return await FindAsync(
                    row => row.BlockNumber == blockNumber.Value.ToString() && 
                         row.Hash == transactionHash && 
                         row.Address == address)
                .ConfigureAwait(false);
        }

        public async Task UpsertAsync(RPC.Eth.DTOs.Transaction transaction, TransactionReceipt transactionReceipt, bool failedCreatingContract, HexBigInteger blockTimestamp, string address, string error = null, bool hasVmStack = false, string newContractAddress = null)
        {
            AddressTransaction tx = new AddressTransaction();

            tx.Map(transaction, address);
            tx.UpdateRowDates();

            await Write(tx);
        }

        protected override ClassMap<AddressTransaction> CreateClassMap()
        {
            return AddressTransactionMap.Instance;
        }
    }

    public class AddressTransactionMap : ClassMap<Entities.AddressTransaction>
    {
        public static AddressTransactionMap Instance = new AddressTransactionMap();

        public AddressTransactionMap()
        {
            AutoMap();
        }
    }
}
