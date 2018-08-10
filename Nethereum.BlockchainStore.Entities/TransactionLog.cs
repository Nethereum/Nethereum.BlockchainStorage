namespace Nethereum.BlockchainStore.Entities
{
    public class TransactionLog: TableRow
    {
        public string TransactionHash { get; set; }
        public long LogIndex { get; set; }
        public string Address { get; set; }
        public string Topics { get; set; }
        public string Topic0 { get; set; }
        public string Data { get; set; }
    }
}