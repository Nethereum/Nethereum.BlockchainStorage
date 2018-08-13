using System.Threading.Tasks;
using Nethereum.BlockchainStore.Repositories;
using Nethereum.Hex.HexTypes;
using Nethereum.RPC.Eth.DTOs;

namespace Nethereum.BlockchainStore.EFCore.Repositories
{
    /// <summary>
    /// An empty stub - the SQL implementation doesn't require this table as the Transaction table already holds the address
    /// </summary>
    public class AddressTransactionRepository : TransactionBaseRepository, IAddressTransactionRepository
    {
        public AddressTransactionRepository(IBlockchainDbContextFactory contextFactory):base(contextFactory){}

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
