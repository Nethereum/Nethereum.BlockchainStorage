namespace Nethereum.RPC.Eth.DTOs
{
    public static class Extensions
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
