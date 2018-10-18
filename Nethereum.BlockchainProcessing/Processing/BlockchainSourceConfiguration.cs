namespace Nethereum.BlockchainProcessing.Processing
{
    public class BlockchainSourceConfiguration
    {
        public BlockchainSourceConfiguration(string blockchainUrl, string name)
        {
            BlockchainUrl = blockchainUrl;
            Name = name;
        }

        public string BlockchainUrl { get; set; }
        public string Name { get; set; }
        public long? MinimumBlockNumber { get; set; }
        public long? FromBlock { get; set; }
        public long? ToBlock { get; set; }
        public bool PostVm { get; set; } = false;
        public bool ProcessBlockTransactionsInParallel { get; set; } = true;
    }
}
