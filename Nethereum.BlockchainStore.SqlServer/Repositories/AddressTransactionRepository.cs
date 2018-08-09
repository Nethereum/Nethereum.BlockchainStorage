using System.Linq;
using Nethereum.BlockchainStore.Repositories;
using Nethereum.Hex.HexTypes;
using Nethereum.RPC.Eth.DTOs;
using System.Threading.Tasks;
using Nethereum.BlockchainStore.SqlServer.Entities;

namespace Nethereum.BlockchainStore.SqlServer.Repositories
{
    public class AddressTransactionRepository : IAddressTransactionRepository
    {
        private readonly IBlockchainDbContextFactory _contextFactory;

        public AddressTransactionRepository(IBlockchainDbContextFactory contextFactory)
        {
            _contextFactory = contextFactory;
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

            var context = _contextFactory.CreateContext();

            var tx = await context.AddressTransactions
                          .FindByBlockNumberAndHashAsync(transaction.BlockNumber, transaction.TransactionHash) ?? new AddressTransaction();

            tx.Address = address;
            tx.BlockHash = transaction.BlockHash;
            tx.Hash = transaction.TransactionHash;
            tx.AddressFrom = transaction.From;
            tx.TransactionIndex = (long)receipt.TransactionIndex.Value;
            tx.Value = transaction.Value.ToString();
            tx.AddressTo = transaction.To ?? string.Empty;
            tx.NewContractAddress = newContractAddress ?? string.Empty;
            tx.BlockNumber = transaction.BlockNumber.Value.ToString();
            tx.Gas = (long)transaction.Gas.Value;
            tx.GasPrice = (long)transaction.GasPrice.Value;
            tx.Input = transaction.Input ?? string.Empty;
            tx.Nonce = (long)transaction.Nonce.Value;
            tx.Failed = failedCreatingContract;
            tx.GasUsed = (long)receipt.GasUsed.Value;
            tx.CumulativeGasUsed = (long)receipt.CumulativeGasUsed.Value;
            tx.HasLog = receipt.Logs.Count > 0;
            tx.TimeStamp = (long)blockTimestamp.Value;
            tx.Error = error ?? string.Empty;
            tx.HasVmStack = hasVmStack;

            if(tx.IsNew())
                context.AddressTransactions.Add(tx);
            else
                context.AddressTransactions.Update(tx);

            await context.SaveChangesAsync();
        }
    }
}
