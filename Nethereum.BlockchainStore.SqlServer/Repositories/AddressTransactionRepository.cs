using Nethereum.BlockchainStore.Repositories;
using Nethereum.BlockchainStore.SqlServer.Entities;
using Nethereum.Hex.HexTypes;
using Nethereum.RPC.Eth.DTOs;
using System.Threading.Tasks;

namespace Nethereum.BlockchainStore.SqlServer.Repositories
{

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

            using (var context = _contextFactory.CreateContext())
            {
                var tx = await context.AddressTransactions
                             .FindByBlockNumberAndHashAsync(transaction.BlockNumber, transaction.TransactionHash) ??
                         new AddressTransaction();

                tx.Address = address;

                MapValues(transaction, tx);
                MapValues(receipt, tx);

                tx.NewContractAddress = newContractAddress ?? string.Empty;
                tx.Failed = failedCreatingContract;
                tx.TimeStamp = (long) blockTimestamp.Value;
                tx.Error = error ?? string.Empty;
                tx.HasVmStack = hasVmStack;

                if (tx.IsNew())
                    context.AddressTransactions.Add(tx);
                else
                    context.AddressTransactions.Update(tx);

                await context.SaveChangesAsync();
            }
        }
    }
}
