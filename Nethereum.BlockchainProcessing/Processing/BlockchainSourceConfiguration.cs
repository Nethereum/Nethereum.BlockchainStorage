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
        public ulong? MinimumBlockNumber { get; set; }
        public ulong? MinimumBlockConfirmations { get; set; }
        public ulong? FromBlock { get; set; }
        public ulong? ToBlock { get; set; }
        public bool PostVm { get; set; } = false;
        public bool ProcessBlockTransactionsInParallel { get; set; } = true;
    }
}
