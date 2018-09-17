using Nethereum.BlockchainStore.Entities;
using Nethereum.BlockchainStore.Repositories;
using Nethereum.Hex.HexTypes;
using Nethereum.RPC.Eth.DTOs;
using System.Threading.Tasks;

namespace Nethereum.BlockchainStore.Csv.Repositories
{
    public class AddressTransactionRepository : IAddressTransactionRepository
    {
        public Task<ITransactionView> FindByAddressBlockNumberAndHashAsync(string addrees, HexBigInteger blockNumber, string transactionHash)
        {
            return Task.FromResult((ITransactionView) null);
        }

        public Task UpsertAsync(RPC.Eth.DTOs.Transaction transaction, TransactionReceipt transactionReceipt, bool failedCreatingContract, HexBigInteger blockTimestamp, string address, string error = null, bool hasVmStack = false, string newContractAddress = null)
        {
            return Task.CompletedTask;
        }
    }
}
