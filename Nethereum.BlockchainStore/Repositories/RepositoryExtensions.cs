using Nethereum.RPC.Eth.DTOs;

namespace Nethereum.BlockchainStore.Repositories
{
    public static class RepositoryExtensions
    {
        public static int TransactionCount(this Block block)
        {
            if (block is BlockWithTransactions b)
                return b.Transactions.Length;

            if (block is BlockWithTransactionHashes bh)
                return bh.TransactionHashes.Length;

            return 0;
        }
    }
}
