using CsvHelper.Configuration;
using Nethereum.BlockchainProcessing.Storage.Entities;
using Nethereum.BlockchainProcessing.Storage.Entities.Mapping;
using Nethereum.BlockchainProcessing.Storage.Repositories;
using Nethereum.Hex.HexTypes;
using Nethereum.RPC.Eth.DTOs;
using System.Threading.Tasks;

namespace Nethereum.BlockchainStore.Csv.Repositories
{
    public class AddressTransactionRepository : CsvRepositoryBase<AddressTransaction>, IAddressTransactionRepository
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

        public async Task UpsertAsync(TransactionReceiptVO transactionReceiptVO, string address, string error = null, string newContractAddress = null)
        {
            var txEntity = transactionReceiptVO.MapToStorageEntityForUpsert(address);
            await Write(txEntity);
        }

        protected override ClassMap<AddressTransaction> CreateClassMap()
        {
            return AddressTransactionMap.Instance;
        }
    }

    public class AddressTransactionMap : ClassMap<AddressTransaction>
    {
        public static AddressTransactionMap Instance = new AddressTransactionMap();

        public AddressTransactionMap()
        {
            AutoMap();
        }
    }
}
