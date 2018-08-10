namespace Nethereum.BlockchainStore.Entities
{
    public class TransactionVmStack: TableRow
    {
        public string Address { get;set; }
        public string TransactionHash { get; set; }
        public string StructLogs { get; set; }
    }
}