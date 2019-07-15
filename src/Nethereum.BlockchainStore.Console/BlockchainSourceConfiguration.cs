using System.Numerics;

namespace Nethereum.BlockchainStore.Console
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
        public uint? MinimumBlockConfirmations { get; set; }
        public BigInteger? FromBlock { get; set; }
        public BigInteger? ToBlock { get; set; }
        public bool PostVm { get; set; } = false;
        public bool ProcessBlockTransactionsInParallel { get; set; } = true;
    }
}
