using Nethereum.BlockchainStore.Entities;
using Nethereum.RPC.Eth.DTOs;
using Transaction = Nethereum.RPC.Eth.DTOs.Transaction;

namespace Nethereum.BlockchainStore.SqlServer.Repositories
{
    public abstract class TransactionBaseRepository: RepositoryBase
    {
        protected TransactionBaseRepository(IBlockchainDbContextFactory contextFactory) : base(contextFactory)
        {
        }

        protected static void MapValues(TransactionReceipt @from, TransactionBase to)
        {
            to.TransactionIndex = (long)@from.TransactionIndex.Value;
            to.GasUsed = (long)@from.GasUsed.Value;
            to.CumulativeGasUsed = (long)@from.CumulativeGasUsed.Value;
            to.HasLog = @from.Logs.Count > 0;
        }

        protected static void MapValues(Transaction @from, TransactionBase to)
        {
            to.BlockHash = @from.BlockHash;
            to.Hash = @from.TransactionHash;
            to.AddressFrom = @from.From;
            to.Value = @from.Value.Value.ToString();
            to.AddressTo = @from.To ?? string.Empty;
            to.BlockNumber = @from.BlockNumber.Value.ToString();
            to.Gas = (long)@from.Gas.Value;
            to.GasPrice = (long)@from.GasPrice.Value;
            to.Input = @from.Input ?? string.Empty;
            to.Nonce = (long)@from.Nonce.Value;
        }
    }
}
