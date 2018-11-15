namespace Nethereum.BlockchainProcessing.Samples
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            //new BlockAndTransactionEnumeration().Run().Wait();
            //new FilterTransactions().Run().Wait();
            //new ListeningForASpecificEvent().Run().Wait();
            //new ListeningForASpecificFunctionCall().Run().Wait();
            new ConditionalTransactionRouting().Run().Wait();
        }
    }
}
