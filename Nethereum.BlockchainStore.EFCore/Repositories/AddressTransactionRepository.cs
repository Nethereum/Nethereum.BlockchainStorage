using System.Threading.Tasks;
using Nethereum.BlockchainStore.Entities;
using Nethereum.BlockchainStore.Repositories;
using Nethereum.Hex.HexTypes;
using Nethereum.RPC.Eth.DTOs;

namespace Nethereum.BlockchainStore.EFCore.Repositories
{
    /// <summary>
    /// An empty stub - the SQL implementation doesn't require this table as the Transaction table already holds the address
    /// </summary>
    public class AddressTransactionRepository : RepositoryBase, IAddressTransactionRepository
    {
        public AddressTransactionRepository(IBlockchainDbContextFactory contextFactory):base(contextFactory){}

        public Task<ITransactionView> FindByAddressBlockNumberAndHashAsync(string addrees, HexBigInteger blockNumber, string transactionHash)
        {
            throw new System.NotImplementedException();
        }

        public async Task UpsertAsync(
            RPC.Eth.DTOs.Transaction transaction, 
            TransactionReceipt receipt, 
            bool failedCreatingContract, 
            HexBigInteger blockTimestamp, 
            string address, 
            string error = null, 
            bool hasVmStack = false, 
            string newContractAddress = null)
        {

            return;
        }
    }
}
